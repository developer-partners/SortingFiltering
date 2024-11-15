using System;
using System.Reflection;

namespace DeveloperPartners.SortingFiltering.EntityFrameworkCore.Helpers.QueryHelper
{
    class PropertyDescriptor
    {
        private Lazy<QueryablePropertyDescriptor> _queryableProperty;

        public string Path { get; set; } = null!;
        public Type OwnerObjectType { get; set; } = null!;
        public PropertyInfo Property { get; set; } = null!;
        public PropertyDescriptor? Child { get; set; }

        public QueryablePropertyDescriptor QueryableProperty
        {
            get
            {
                return _queryableProperty.Value;
            }
        }

        public PropertyDescriptor()
        {
            _queryableProperty = new Lazy<QueryablePropertyDescriptor>(GetQueryableProperty);
        }

        private QueryablePropertyDescriptor GetQueryableProperty()
        {
            if (this.Child == null)
            {
                return new QueryablePropertyDescriptor(this.Property);
            }

            return this.Child.GetQueryableProperty();
        }
    }
}