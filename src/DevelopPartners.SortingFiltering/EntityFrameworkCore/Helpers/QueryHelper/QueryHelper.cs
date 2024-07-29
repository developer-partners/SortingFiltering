
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DeveloperPartners.SortingFiltering.EntityFrameworkCore.Helpers.QueryHelper
{
    static class QueryHelper
    {
        private static IOrderedQueryable<T> OrderById<T>(IQueryable<T> itemList, Type tableType)
        {
            var idProperty = tableType.GetProperty("Id");

            if (idProperty != null)
            {
                var parentExpression = Expression.Parameter(tableType);
                var orderByExpression = Expression.Property(parentExpression, "Id");
                var convertedExpression = Expression.Convert(orderByExpression, typeof(object));

                var lambdaExpression = Expression.Lambda<Func<T, object>>(convertedExpression, parentExpression);

                return itemList.OrderBy(lambdaExpression);
            }

            return (IOrderedQueryable<T>)itemList;
        }

        public static IOrderedQueryable<T> OrderBy<T>(IQueryable<T> itemList, IDictionary<string, SortOperator> sortColumns)
        {
            var tableType = typeof(T);

            if (sortColumns.IsNullOrEmpty())
            {
                return OrderById(itemList, tableType);
            }

            var count = 0;

            foreach (var column in sortColumns)
            {
                var property = GetProperty(tableType, column.Key);

                if (property != null)
                {
                    var expression = GetOrderByExpression<T, object>(property);
                    itemList = OrderBy(itemList, column.Value, expression, count++);
                }
            }

            return (IOrderedQueryable<T>)itemList;
        }

        private static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(IQueryable<TSource> itemList, SortOperator @operator, Expression<Func<TSource, TKey>> expression, int count)
        {
            var orderedList = itemList as IOrderedQueryable<TSource>;

            if (orderedList != null && count > 0)
            {
                return @operator == SortOperator.Asc
                  ? orderedList.ThenBy(expression)
                  : orderedList.ThenByDescending(expression);
            }

            return @operator == SortOperator.Asc
              ? itemList.OrderBy(expression)
              : itemList.OrderByDescending(expression);
        }

        private static PropertyInfo GetPropertyIgnoreCase(Type ownerObjectType, string propertyName)
        {
            var property = ownerObjectType.GetProperty(propertyName);

            if (property == null)
            {
                try
                {
                    // If we can't get the property, let's try to query it with IgnoreCase query.
                    return ownerObjectType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                }
                catch (AmbiguousMatchException)
                {
                }
            }

            return property;
        }

        private static PropertyDescriptor GetProperty(Type ownerObjectType, string path)
        {
            // Path can be a simple property such as "Name" or a nested property such as "Business.Name"
            var hierarchy = new Queue<string>(path.Split('.'));
            return GetProperty(ownerObjectType, hierarchy, path);
        }

        private static PropertyDescriptor GetProperty(Type ownerObjectType, Queue<string> hierarchy, string path)
        {
            if (hierarchy.TryDequeue(out var propertyName))
            {
                var property = GetPropertyIgnoreCase(ownerObjectType, propertyName);

                if (property != null)
                {
                    return new PropertyDescriptor
                    {
                        Path = path,
                        OwnerObjectType = ownerObjectType,
                        Property = property,
                        Child = GetProperty(property.PropertyType, hierarchy, path)
                    };
                }
            }

            return null;
        }

        private static Expression GetExpression(PropertyDescriptor propertyDescriptor, Expression parentExpression)
        {
            Expression expression = Expression.Property(parentExpression, propertyDescriptor.Property.Name);

            if (propertyDescriptor.Child != null)
            {
                // If there is a child property, we have to make our expresison to be something like this:
                // EF.Property<string>(EF.Property<Business>(i, "Business"), "Name")
                expression = GetExpression(propertyDescriptor.Child, expression);
            }

            if (propertyDescriptor.QueryableProperty.IsNullable)
            {
                expression = Expression.Convert(expression, propertyDescriptor.QueryableProperty.UnderlyingPropertyType);
            }

            return expression;
        }

        private static Expression<Func<TSource, TKey>> GetOrderByExpression<TSource, TKey>(PropertyDescriptor propertyDescriptor)
        {
            var parameterExpression = Expression.Parameter(propertyDescriptor.OwnerObjectType);
            var expression = GetExpression(propertyDescriptor, parameterExpression);

            var convertedExpression = Expression.Convert(expression, typeof(TKey));

            return Expression.Lambda<Func<TSource, TKey>>(convertedExpression, parameterExpression);
        }

        /// <summary>
        /// Use this for parametrizing EF queries to cache query execution plans and avoid SQL injection attacks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static MemberExpression CreatePropertyExpresison<T>(T value)
        {
            return CreatePropertyExpresison(value, typeof(T));
        }

        /// <summary>
        /// Use this for parametrizing EF queries to cache query execution plans and avoid SQL injection attacks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private static MemberExpression CreatePropertyExpresison(object value, Type propertyType)
        {
            var convertedType = propertyType.ChangeType(value);

            var wrapperType = typeof(WrappedObj<>).MakeGenericType(propertyType);

            var constructor = wrapperType
              .GetConstructors()
              .First();

            var wrapper = constructor.Invoke(new object[]
            {
                convertedType
            });

            return Expression.Property(
                Expression.Constant(wrapper),
                "Value"
            );
        }

        private static Expression GetStringQueryExpression(QueryProperty propertyQuery, Expression propertyAccessExpression)
        {
            switch (propertyQuery.ComparisonOperator)
            {
                case ComparisonOperator.StW:
                    return Expression.Call(
                      propertyAccessExpression,
                      typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) }),
                      CreatePropertyExpresison(propertyQuery.Value)
                    );
                case ComparisonOperator.NotStW:
                    var startsWith = Expression.Call(
                      propertyAccessExpression,
                      typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) }),
                      CreatePropertyExpresison(propertyQuery.Value)
                    );

                    return Expression.Not(startsWith);
                case ComparisonOperator.EndW:
                    return Expression.Call(
                      propertyAccessExpression,
                      typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) }),
                      CreatePropertyExpresison(propertyQuery.Value)
                    );
                case ComparisonOperator.NotEndW:
                    var endsWith = Expression.Call(
                      propertyAccessExpression,
                      typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) }),
                      CreatePropertyExpresison(propertyQuery.Value)
                    );

                    return Expression.Not(endsWith);
                case ComparisonOperator.Ct:
                    return Expression.Call(
                      propertyAccessExpression,
                      typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) }),
                      CreatePropertyExpresison(propertyQuery.Value)
                    );
                case ComparisonOperator.NotCt:
                    var contains = Expression.Call(
                      propertyAccessExpression,
                      typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) }),
                      CreatePropertyExpresison(propertyQuery.Value)
                    );

                    return Expression.Not(contains);
                case ComparisonOperator.NotEq:
                    return Expression.NotEqual(propertyAccessExpression, CreatePropertyExpresison(propertyQuery.Value));
            }

            return Expression.Equal(propertyAccessExpression, CreatePropertyExpresison(propertyQuery.Value));
        }

        private static ExpressionHelper GetNumericExpressionHelper(PropertyDescriptor propertyDescriptor, QueryProperty propertyQuery, Expression propertyAccessExpression)
        {
            if (!decimal.TryParse(propertyQuery.Value, out var test))
            {
                return null;
            }

            // If it's a number with decimal points, we round the number for comparison.
            // For example, if the value is 4.65, we round it to 5.
            if (propertyDescriptor.QueryableProperty.UnderlyingPropertyType.IsDecimalNumber())
            {
                var convertMethodInfo = typeof(Convert).GetMethod(nameof(Convert.ToDecimal), new Type[]
                {
                      propertyDescriptor.QueryableProperty.UnderlyingPropertyType
                });

                var roundMethodInfo = typeof(Math).GetMethod(nameof(Math.Round), new Type[]
                {
                  typeof(decimal),
                  typeof(int)
                });

                var convertLeftExpression = Expression.Call(convertMethodInfo, propertyAccessExpression);
                var convertRightExpression = Expression.Call(convertMethodInfo, CreatePropertyExpresison(propertyQuery.ParsedValue, propertyDescriptor.QueryableProperty.UnderlyingPropertyType));

                // This will result to something like this:
                // Math.Round(Convert.ToDecimal(e.SomeProperty, 0))
                return new ExpressionHelper
                {
                    Left = Expression.Call(
                    roundMethodInfo,
                    convertLeftExpression,
                    Expression.Constant(0)
                  ),
                    Right = Expression.Call(
                    roundMethodInfo,
                    convertRightExpression,
                    Expression.Constant(0)
                  )
                };
            }

            return new ExpressionHelper
            {
                Left = propertyAccessExpression,
                Right = CreatePropertyExpresison(propertyQuery.ParsedValue, propertyDescriptor.QueryableProperty.UnderlyingPropertyType)
            };
        }

        private static Expression GetNumericQueryExpression(PropertyDescriptor propertyDescriptor, QueryProperty propertyQuery, Expression propertyAccessExpression)
        {
            var numericHelper = GetNumericExpressionHelper(propertyDescriptor, propertyQuery, propertyAccessExpression);

            if (numericHelper != null)
            {
                switch (propertyQuery.ComparisonOperator)
                {
                    case ComparisonOperator.Lt:
                        return Expression.LessThan(numericHelper.Left, numericHelper.Right);
                    case ComparisonOperator.Lte:
                        return Expression.LessThanOrEqual(numericHelper.Left, numericHelper.Right);
                    case ComparisonOperator.Gt:
                        return Expression.GreaterThan(numericHelper.Left, numericHelper.Right);
                    case ComparisonOperator.Gte:
                        return Expression.GreaterThanOrEqual(numericHelper.Left, numericHelper.Right);
                    case ComparisonOperator.NotEq:
                        return Expression.NotEqual(numericHelper.Left, numericHelper.Right);
                }

                return Expression.Equal(numericHelper.Left, numericHelper.Right);
            }

            return null;
        }

        private static DateExpressionHelper GetDateExpresionHelper(PropertyDescriptor propertyDescriptor, QueryProperty propertyQuery, Expression propertyAccessExpression)
        {
            if (DateTime.TryParse(propertyQuery.Value, out var dateValue))
            {
                var clientTimeZone = propertyQuery.ClientTimeZone ?? TimeZoneInfo.Utc;

                var utcOffset = clientTimeZone.GetUtcOffset(DateTime.UtcNow);
                var utcOfsetMinutes = -1 * utcOffset.TotalMinutes;

                var startTime = dateValue.Date.AddMinutes(utcOfsetMinutes);
                var endTime = dateValue.Date.AddDays(1).AddSeconds(-1).AddMinutes(utcOfsetMinutes);

                return new DateExpressionHelper
                {
                    StartTime = new ExpressionHelper
                    {
                        Left = propertyAccessExpression,
                        Right = CreatePropertyExpresison(startTime, propertyDescriptor.QueryableProperty.UnderlyingPropertyType)
                    },
                    EndTime = new ExpressionHelper
                    {
                        Left = propertyAccessExpression,
                        Right = CreatePropertyExpresison(endTime, propertyDescriptor.QueryableProperty.UnderlyingPropertyType)
                    }
                };
            }

            return null;
        }

        private static Expression GetDateQueryExpression(PropertyDescriptor propertyDescriptor, QueryProperty propertyQuery, Expression propertyAccessExpression)
        {
            var dateHelper = GetDateExpresionHelper(propertyDescriptor, propertyQuery, propertyAccessExpression);

            if (dateHelper != null)
            {
                switch (propertyQuery.ComparisonOperator)
                {
                    case ComparisonOperator.Lt:
                        return Expression.LessThan(dateHelper.StartTime.Left, dateHelper.StartTime.Right);
                    case ComparisonOperator.Lte:
                        return Expression.LessThanOrEqual(dateHelper.EndTime.Left, dateHelper.EndTime.Right);
                    case ComparisonOperator.Gt:
                        return Expression.GreaterThan(dateHelper.EndTime.Left, dateHelper.EndTime.Right);
                    case ComparisonOperator.Gte:
                        return Expression.GreaterThanOrEqual(dateHelper.StartTime.Left, dateHelper.StartTime.Right);
                    case ComparisonOperator.NotEq:
                        return Expression.OrElse(
                            Expression.LessThan(dateHelper.StartTime.Left, dateHelper.StartTime.Right),
                            Expression.GreaterThan(dateHelper.EndTime.Left, dateHelper.EndTime.Right)
                        );
                }

                return Expression.AndAlso(
                    Expression.GreaterThanOrEqual(dateHelper.StartTime.Left, dateHelper.StartTime.Right),
                    Expression.LessThanOrEqual(dateHelper.EndTime.Left, dateHelper.EndTime.Right)
                );
            }

            return null;
        }

        private static Expression GetNullQueryExpression(PropertyDescriptor propertyDescriptor, QueryProperty propertyQuery, Expression propertyAccessExpression)
        {
            if (propertyQuery.ComparisonOperator == ComparisonOperator.NotEq)
            {
                return Expression.NotEqual(Expression.Convert(propertyAccessExpression, typeof(object)), Expression.Constant(null));
            }

            return Expression.Equal(Expression.Convert(propertyAccessExpression, typeof(object)), Expression.Constant(null));
        }

        private static Expression GetOtherQueryExpression(PropertyDescriptor propertyDescriptor, QueryProperty propertyQuery, Expression propertyAccessExpression)
        {
            if (propertyQuery.ComparisonOperator == ComparisonOperator.NotEq)
            {
                return Expression.NotEqual(propertyAccessExpression, CreatePropertyExpresison(propertyQuery.ParsedValue, propertyDescriptor.QueryableProperty.UnderlyingPropertyType));
            }

            return Expression.Equal(propertyAccessExpression, CreatePropertyExpresison(propertyQuery.ParsedValue, propertyDescriptor.QueryableProperty.UnderlyingPropertyType));
        }

        private static Expression GetQueryExpression(PropertyDescriptor propertyDescriptor, QueryProperty propertyQuery, Expression propertyAccessExpression)
        {
            if (propertyQuery.IsValueNull)
            {
                return GetNullQueryExpression(propertyDescriptor, propertyQuery, propertyAccessExpression);
            }

            if (propertyDescriptor.QueryableProperty.UnderlyingPropertyType == typeof(string))
            {
                return GetStringQueryExpression(propertyQuery, propertyAccessExpression);
            }

            if (propertyDescriptor.QueryableProperty.UnderlyingPropertyType.IsNumeric())
            {
                return GetNumericQueryExpression(propertyDescriptor, propertyQuery, propertyAccessExpression);
            }

            if (propertyDescriptor.QueryableProperty.UnderlyingPropertyType == typeof(DateTime))
            {
                return GetDateQueryExpression(propertyDescriptor, propertyQuery, propertyAccessExpression);
            }

            return GetOtherQueryExpression(propertyDescriptor, propertyQuery, propertyAccessExpression);
        }

        private static QueryExpressionDescriptor<TSource> GetQueryDescriptor<TSource>(Type tableType, PropertyDescriptor propertyDescriptor, QueryProperty propertyQuery, Expression parentExpression)
        {
            var propertyAccessExpression = GetExpression(propertyDescriptor, parentExpression);

            var expressionDescriptor = new QueryExpressionDescriptor<TSource>
            {
                QueryProperty = propertyQuery,
                Expression = GetQueryExpression(propertyDescriptor, propertyQuery, propertyAccessExpression)
            };

            if (!propertyQuery.Children.IsNullOrEmpty())
            {
                expressionDescriptor.Children = new List<QueryExpressionDescriptor<TSource>>();

                foreach (var child in propertyQuery.Children)
                {
                    if (string.IsNullOrWhiteSpace(child.ColumnName))
                    {
                        // This way a nested query will get its parent column name if it wasn't provided.
                        child.ColumnName = propertyQuery.ColumnName;
                    }

                    var childExpression = GetQueryDescriptor<TSource>(tableType, child, parentExpression);

                    if (childExpression != null)
                    {
                        expressionDescriptor.Children.Add(childExpression);
                    }
                }
            }

            return expressionDescriptor;
        }

        private static QueryExpressionDescriptor<TSource> GetQueryDescriptor<TSource>(Type tableType, QueryProperty propertyQuery, Expression parentExpression)
        {
            if (propertyQuery != null && (!string.IsNullOrWhiteSpace(propertyQuery.Value) || propertyQuery.IsValueNull))
            {
                if (string.IsNullOrWhiteSpace(propertyQuery.ColumnName))
                {
                    throw new ArgumentNullException($"Filter property {nameof(propertyQuery.ColumnName)} cannot be empty. Please set a value for it.");
                }

                var property = GetProperty(tableType, propertyQuery.ColumnName);

                if (property != null)
                {
                    var descriptor = GetQueryDescriptor<TSource>(tableType, property, propertyQuery, parentExpression);

                    if (descriptor?.Expression != null)
                    {
                        return descriptor;
                    }
                }
            }

            return null;
        }

        private static Expression<Func<TSource, bool>> GetWhereClause<TSource>(IEnumerable<QueryExpressionDescriptor<TSource>> expressionDescriptors, ParameterExpression parentExpression)
        {
            var result = expressionDescriptors.First().FlattenHierarchy();

            foreach (var descriptor in expressionDescriptors.Skip(1))
            {
                var right = descriptor.FlattenHierarchy();

                result = descriptor.QueryProperty.LogicalOperator == LogicalOperator.And
                  ? Expression.AndAlso(result, right)
                  : Expression.OrElse(result, right);
            }

            return Expression.Lambda<Func<TSource, bool>>(result, parentExpression);
        }

        public static IQueryable<T> Where<T>(IQueryable<T> itemList, QueryFilter filter)
        {
            if (!filter.IsNullOrEmpty())
            {
                var tableType = typeof(T);
                var parentExpression = Expression.Parameter(tableType);

                var expressionDescriptors = filter
                  .Select(queryProperty => GetQueryDescriptor<T>(tableType, queryProperty, parentExpression))
                  .Where(e => e != null);

                if (!expressionDescriptors.IsNullOrEmpty())
                {
                    var whereClause = GetWhereClause(expressionDescriptors, parentExpression);
                    itemList = itemList.Where(whereClause);
                }
            }

            return itemList;
        }
    }

    class WrappedObj<TValue>
    {
        public TValue Value { get; set; }

        public WrappedObj(TValue value)
        {
            this.Value = value;
        }
    }

    class ExpressionHelper
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }

    class DateExpressionHelper
    {
        public ExpressionHelper StartTime { get; set; }
        public ExpressionHelper EndTime { get; set; }
    }
}