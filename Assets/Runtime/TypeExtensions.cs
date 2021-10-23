using System;
using System.Linq;

namespace Fp.Utility
{
    public static class TypeExtensions
    {
        /// <summary>
        ///     Determines whether the <paramref name="genericType" /> is assignable from
        ///     <paramref name="givenType" /> taking into account generic definitions
        /// </summary>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType == null || genericType == null)
            {
                return false;
            }

            return givenType == genericType
                || givenType.MapsToGenericTypeDefinition(genericType)
                || givenType.HasInterfaceThatMapsToGenericTypeDefinition(genericType)
                || givenType.BaseType.IsAssignableToGenericType(genericType);
        }

        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return Nullable.GetUnderlyingType(type).IsNumericType();
                    }

                    return false;
                default:
                    return false;
            }
        }

        private static bool HasInterfaceThatMapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return givenType
                   .GetInterfaces()
                   .Where(it => it.IsGenericType)
                   .Any(it => it.GetGenericTypeDefinition() == genericType);
        }

        private static bool MapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return genericType.IsGenericTypeDefinition
                && givenType.IsGenericType
                && givenType.GetGenericTypeDefinition() == genericType;
        }
    }
}