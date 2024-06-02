
using System;
using System.Collections;

namespace DeveloperPartners.SortingFiltering
{
    internal static class TypeExtensions
    {
        public static bool IsEnumarable(this Type t)
        {
            if (typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(string))
            {
                return true;
            }

            return false;
        }

        public static bool IsClass(this Type t)
        {
            if (t.IsClass && !t.IsEnumarable() && t != typeof(string))
            {
                return true;
            }

            return false;
        }

        public static bool IsNumeric(this Type type)
        {
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
            }

            return false;
        }

        public static bool IsDecimalNumber(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
            }

            return false;
        }

        public static bool IsEnum(this Type t)
        {
            var typeToCheck = Nullable.GetUnderlyingType(t) ?? t;
            return typeToCheck.IsEnum;
        }

        public static object ParseEnum(this Type t, object value)
        {
            var typeToChange = Nullable.GetUnderlyingType(t) ?? t;

            if (value != null)
            {
                if (value.GetType() == typeof(string))
                {
                    if (Enum.TryParse(typeToChange, value.ToString(), out var result))
                    {
                        return result;
                    }

                    return Enum.ToObject(typeToChange, -1);
                }

                return Enum.ToObject(typeToChange, value);
            }

            return Enum.ToObject(typeToChange, -1);
        }

        public static object ChangeType(this Type t, object value)
        {
            var typeToChange = Nullable.GetUnderlyingType(t) ?? t;

            if (typeToChange == typeof(Guid))
            {
                return value != null
                    ? new Guid(value.ToString())
                    : Guid.Empty;
            }

            if (typeToChange == typeof(Guid?))
            {
                return value != null
                    ? new Guid(value.ToString())
                    : default(Guid?);
            }

            if (typeToChange.IsEnum())
            {
                return t.ParseEnum(value);
            }

            return value == null
                ? null
                : Convert.ChangeType(value, typeToChange);
        }

        public static bool IsNullable(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }

            return false;
        }
    }
}