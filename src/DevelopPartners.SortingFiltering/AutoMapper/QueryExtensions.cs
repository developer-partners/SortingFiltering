
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Internal;

namespace DeveloperPartners.SortingFiltering.AutoMapper
{
    public static class QueryExtensions
    {
        private static void ConvertComplexPropertyParts(IMapper mapper, Queue<string> queue, PropertyMap propertyMap, ICollection<string> list)
        {
            var nestedMap = mapper
              .ConfigurationProvider
              .Internal()
              .FindTypeMapFor(propertyMap.SourceType, propertyMap.DestinationType);

            if (nestedMap != null)
            {
                if (propertyMap.SourceMember != null)
                {
                    list.Add(propertyMap.SourceMember.Name);
                }

                ConvertPropertyParts(mapper, queue, nestedMap, list);
            }
        }

        private static Type GetMemberInfoType(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                default:
                    throw new ArgumentException("The member must be either a field or property.");
            }
        }

        private static void ConvertPropertyParts(IMapper mapper, Queue<string> queue, TypeMap typeMap, ICollection<string> list)
        {
            if (queue.TryDequeue(out var propertyName))
            {
                var propertyMap = typeMap
                  .PropertyMaps
                  .FirstOrDefault(m => m.DestinationName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

                if (propertyMap != null)
                {
                    if (propertyMap.SourceType.IsClass && propertyMap.SourceType != typeof(string))
                    {
                        ConvertComplexPropertyParts(mapper, queue, propertyMap, list);
                    }
                    else if (propertyMap.CustomMapExpression != null)
                    {
                        var sourceMemberName = propertyMap.CustomMapExpression.GetMemberName(true);
                        list.Add(sourceMemberName);
                    }
                    else
                    {
                        list.Add(propertyMap.SourceMember.Name);
                    }
                }
            }
        }

        private static string ConvertPropertyName(IMapper mapper, string propertyPath, TypeMap typeMap)
        {
            if (!string.IsNullOrWhiteSpace(propertyPath))
            {
                var parts = propertyPath.Split('.');
                var queue = new Queue<string>(parts);
                var convertedParts = new List<string>();

                ConvertPropertyParts(mapper, queue, typeMap, convertedParts);

                if (convertedParts.Count > 0)
                {
                    return string.Join('.', convertedParts);
                }
            }

            return string.Empty;
        }

        private static IDictionary<string, SortOperator> ConvertSortProperties(Query query, IMapper mapper, TypeMap typeMap)
        {
            var result = new Dictionary<string, SortOperator>();

            foreach (var sortProperty in query.Sort)
            {
                var propertyName = ConvertPropertyName(mapper, sortProperty.Key, typeMap);

                if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    result[propertyName] = sortProperty.Value;
                }
                else
                {
                    result[sortProperty.Key] = sortProperty.Value;
                }
            }

            return result;
        }

        public static IDictionary<string, SortOperator> ConvertSortProperties<TModel, TDto>(this Query query, IMapper mapper)
            where TModel : class
            where TDto : class
        {
            var typeMap = mapper
                .ConfigurationProvider
                .Internal()
                .FindTypeMapFor<TModel, TDto>();

            if (typeMap != null)
            {
                return ConvertSortProperties(query, mapper, typeMap);
            }

            return query.Sort;
        }

        private static QueryFilter ConvertFilterProperties(IMapper mapper, TypeMap typeMap, IEnumerable<QueryProperty> filterToConvert, QueryProperty parent)
        {
            var result = new QueryFilter();

            foreach (var property in filterToConvert)
            {
                // This way a nested query will get its parent column name if it wasn't provided.
                var propertyNameToConvert = string.IsNullOrWhiteSpace(property.ColumnName) && parent != null
                  ? parent.ColumnName
                  : property.ColumnName;

                var propertyName = ConvertPropertyName(mapper, propertyNameToConvert, typeMap);

                result.Add(new QueryProperty
                {
                    ColumnName = string.IsNullOrWhiteSpace(propertyName) ? property.ColumnName : propertyName,
                    ComparisonOperator = property.ComparisonOperator,
                    LogicalOperator = property.LogicalOperator,
                    Value = property.Value,
                    Children = ConvertFilterProperties(mapper, typeMap, property.Children, property),
                    IsValueNull = property.IsValueNull
                });
            }

            return result;
        }

        public static QueryFilter ConvertFilterProperties<TModel, TDto>(this Query query, IMapper mapper)
            where TModel : class
            where TDto : class
        {
            var typeMap = mapper
                .ConfigurationProvider
                .Internal()
                .FindTypeMapFor<TModel, TDto>();

            if (typeMap != null)
            {
                return ConvertFilterProperties(mapper, typeMap, query.Filter, null);
            }

            return query.Filter;
        }
    }
}