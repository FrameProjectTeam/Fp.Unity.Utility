using System;
using System.Runtime.CompilerServices;

using Unity.Collections.LowLevel.Unsafe;

namespace Fp.Utility
{
    public static class EnumConverter<TEnum>
        where TEnum : struct, Enum
    {
        private static readonly TypeCode Type;

        static EnumConverter()
        {
            Type underlyingType = typeof(TEnum).GetEnumUnderlyingType();
            Type = System.Type.GetTypeCode(underlyingType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TCast UnsafeCastTo<TCast>(ref TEnum value)
            where TCast : struct
        {
            return UnsafeUtility.As<TEnum, TCast>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum UnsafeCastFrom<TCast>(ref TCast value)
            where TCast : struct
        {
            return UnsafeUtility.As<TCast, TEnum>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TCast? UnsafeCastTo<TCast>(TEnum? value)
            where TCast : struct
        {
            if (!value.HasValue)
            {
                return default;
            }

            TEnum enumVal = value.Value;
            return UnsafeCastTo<TCast>(ref enumVal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum? UnsafeCastFrom<TCast>(TCast? value)
            where TCast : struct
        {
            if (!value.HasValue)
            {
                return default;
            }

            TCast val = value.Value;
            return UnsafeCastFrom(ref val);
        }

        public static byte EnumToByte(TEnum value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                    return UnsafeCastTo<byte>(ref value);
                default:
                    ThrowNotSupportedConversion<byte>();
                    break;
            }

            return default;
        }

        public static byte? EnumToByteNullable(TEnum? value)
        {
            return value.HasValue ? EnumToByte(value.Value) : default(byte?);
        }

        public static TEnum ByteToEnum(byte value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                    return UnsafeCastFrom(ref value);
                default:
                    ThrowNotSupportedConversion<byte>();
                    break;
            }

            return default;
        }

        public static TEnum? ByteToEnumNullable(byte? value)
        {
            return value.HasValue ? ByteToEnum(value.Value) : default;
        }

        public static short EnumToInt16(TEnum value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                    return UnsafeCastTo<byte>(ref value);
                case TypeCode.Int16:
                    return UnsafeCastTo<short>(ref value);
                case TypeCode.SByte:
                    return UnsafeCastTo<sbyte>(ref value);
                default:
                    ThrowNotSupportedConversion<short>();
                    break;
            }

            return default;
        }

        public static short? EnumToInt16Nullable(TEnum? value)
        {
            return value.HasValue ? EnumToInt16(value.Value) : default;
        }

        public static TEnum Int16ToEnum(short value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                    return UnsafeCastFrom(ref value);
                default:
                    ThrowNotSupportedConversion<short>();
                    break;
            }

            return default;
        }

        public static TEnum? Int16ToEnumNullable(short? value)
        {
            return value.HasValue ? Int16ToEnum(value.Value) : default;
        }

        public static int EnumToInt32(TEnum value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                    return UnsafeCastTo<byte>(ref value);
                case TypeCode.Int16:
                    return UnsafeCastTo<short>(ref value);
                case TypeCode.Int32:
                    return UnsafeCastTo<int>(ref value);
                case TypeCode.SByte:
                    return UnsafeCastTo<sbyte>(ref value);
                case TypeCode.UInt16:
                    return UnsafeCastTo<ushort>(ref value);
                default:
                    ThrowNotSupportedConversion<byte>();
                    break;
            }

            return default;
        }

        public static int? EnumToInt32Nullable(TEnum? value)
        {
            return value.HasValue ? EnumToInt32(value.Value) : default;
        }

        public static TEnum Int32ToEnum(int value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                    return UnsafeCastFrom(ref value);
                default:
                    ThrowNotSupportedConversion<byte>();
                    break;
            }

            return default;
        }

        public static TEnum? Int32ToEnumNullable(int? value)
        {
            return value.HasValue ? Int32ToEnum(value.Value) : default;
        }

        public static long EnumToInt64(TEnum value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                    return UnsafeCastTo<byte>(ref value);
                case TypeCode.Int16:
                    return UnsafeCastTo<short>(ref value);
                case TypeCode.Int32:
                    return UnsafeCastTo<int>(ref value);
                case TypeCode.Int64:
                    return UnsafeCastTo<long>(ref value);
                case TypeCode.SByte:
                    return UnsafeCastTo<sbyte>(ref value);
                case TypeCode.UInt16:
                    return UnsafeCastTo<ushort>(ref value);
                case TypeCode.UInt32:
                    return UnsafeCastTo<uint>(ref value);
                default:
                    ThrowNotSupportedConversion<byte>();
                    break;
            }

            return default;
        }

        public static long? EnumToInt64Nullable(TEnum? value)
        {
            return value.HasValue ? EnumToInt64(value.Value) : default;
        }

        public static TEnum Int64ToEnum(long value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                    return UnsafeCastFrom(ref value);
                default:
                    ThrowNotSupportedConversion<byte>();
                    break;
            }

            return default;
        }

        public static TEnum? Int64ToEnumNullable(long? value)
        {
            return value.HasValue ? Int64ToEnum(value.Value) : default;
        }

        public static ulong EnumToUInt64(TEnum value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                    return UnsafeCastTo<byte>(ref value);
                case TypeCode.UInt16:
                    return UnsafeCastTo<ushort>(ref value);
                case TypeCode.UInt32:
                    return UnsafeCastTo<uint>(ref value);
                case TypeCode.UInt64:
                    return UnsafeCastTo<ulong>(ref value);
                default:
                    ThrowNotSupportedConversion<byte>();
                    break;
            }

            return default;
        }

        public static ulong? EnumToUInt64Nullable(TEnum? value)
        {
            return value.HasValue ? EnumToUInt64(value.Value) : default;
        }

        public static TEnum UInt64ToEnum(ulong value)
        {
            switch (Type)
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return UnsafeCastFrom(ref value);
                default:
                    ThrowNotSupportedConversion<byte>();
                    break;
            }

            return default;
        }

        public static TEnum? UInt64ToEnumNullable(ulong? value)
        {
            return value.HasValue ? UInt64ToEnum(value.Value) : default;
        }

        private static void ThrowNotSupportedConversion<TTarget>()
            where TTarget : struct
        {
            Type underlyingType = typeof(TEnum).GetEnumUnderlyingType();
            throw new InvalidCastException($"Can't convert enum to {typeof(TTarget).Name} UnderlyingType {underlyingType}");
        }
    }
}