
using System;
using System.Reflection;

namespace DeveloperPartners.SortingFiltering.EntityFrameworkCore.Helpers.QueryHelper
{
    class QueryablePropertyDescriptor
    {
        public Type PropertyType { get; set; }
        public Type UnderlyingPropertyType { get; private set; }

        public bool IsNullable
        {
            get
            {
                return this.PropertyType.IsNullable();
            }
        }

        public QueryablePropertyDescriptor(PropertyInfo propertyInfo)
        {
            this.PropertyType = propertyInfo.PropertyType;
            this.UnderlyingPropertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
        }
    }
}