using System;
using System.Collections.Generic;
using System.Linq;

namespace OWML.Utils
{
    public static partial class EnumUtils
    {
        public static object Parse(Type enumType, string value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            try
            {
                return System.Enum.Parse(enumType, value);
            }
            catch
            {
                return null;
            }
        }

        public static object Parse(Type enumType, string value, bool ignoreCase)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            try
            {
                return System.Enum.Parse(enumType, value, ignoreCase);
            }
            catch
            {
                return null;
            }
        }

        public static object FromObject(Type enumType, object value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        public static object FromObject(Type enumType, sbyte value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        public static object FromObject(Type enumType, byte value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        public static object FromObject(Type enumType, short value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        public static object FromObject(Type enumType, ushort value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        public static object FromObject(Type enumType, int value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        public static object FromObject(Type enumType, uint value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        public static object FromObject(Type enumType, long value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        public static object FromObject(Type enumType, ulong value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        public static string[] GetNames(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.GetNames(enumType);
        }

        public static object[] GetValues(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.GetValues(enumType).Cast<object>().ToArray();
        }

        public static bool IsDefined(Type enumType, object value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            try
            {
                return System.Enum.IsDefined(enumType, value);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDefined(Type enumType, sbyte value) => IsDefined(enumType, (object)value);
        public static bool IsDefined(Type enumType, byte value) => IsDefined(enumType, (object)value);
        public static bool IsDefined(Type enumType, short value) => IsDefined(enumType, (object)value);
        public static bool IsDefined(Type enumType, ushort value) => IsDefined(enumType, (object)value);
        public static bool IsDefined(Type enumType, int value) => IsDefined(enumType, (object)value);
        public static bool IsDefined(Type enumType, uint value) => IsDefined(enumType, (object)value);
        public static bool IsDefined(Type enumType, long value) => IsDefined(enumType, (object)value);
        public static bool IsDefined(Type enumType, ulong value) => IsDefined(enumType, (object)value);
        public static bool IsDefined(Type enumType, string value) => IsDefined(enumType, (object)value);

        public static object GetMinValue(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return GetValues(enumType).Min();
        }

        public static object GetMaxValue(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return GetValues(enumType).Max();
        }

        public static T Parse<T>(string value, T errorReturn = default) where T : Enum
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            catch
            {
                return errorReturn;
            }
        }

        public static T Parse<T>(string value, bool ignoreCase, T errorReturn = default) where T : Enum
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
            catch
            {
                return errorReturn;
            }
        }

        public static T FromObject<T>(object value) where T : Enum => (T)Enum.ToObject(typeof(T), value);
        public static T FromObject<T>(sbyte value) where T : Enum => (T)Enum.ToObject(typeof(T), value);
        public static T FromObject<T>(byte value) where T : Enum => (T)Enum.ToObject(typeof(T), value);
        public static T FromObject<T>(short value) where T : Enum => (T)Enum.ToObject(typeof(T), value);
        public static T FromObject<T>(ushort value) where T : Enum => (T)Enum.ToObject(typeof(T), value);
        public static T FromObject<T>(int value) where T : Enum => (T)Enum.ToObject(typeof(T), value);
        public static T FromObject<T>(uint value) where T : Enum => (T)Enum.ToObject(typeof(T), value);
        public static T FromObject<T>(long value) where T : Enum => (T)Enum.ToObject(typeof(T), value);
        public static T FromObject<T>(ulong value) where T : Enum => (T)Enum.ToObject(typeof(T), value);

        public static string[] GetNames<T>() where T : Enum => Enum.GetNames(typeof(T));

        public static T[] GetValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().ToArray();

        public static bool IsDefined<T>(object value) where T : Enum
        {
            try
            {
                return Enum.IsDefined(typeof(T), value);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDefined<T>(sbyte value) where T : Enum => IsDefined<T>((object)value);
        public static bool IsDefined<T>(byte value) where T : Enum => IsDefined<T>((object)value);
        public static bool IsDefined<T>(short value) where T : Enum => IsDefined<T>((object)value);
        public static bool IsDefined<T>(ushort value) where T : Enum => IsDefined<T>((object)value);
        public static bool IsDefined<T>(int value) where T : Enum => IsDefined<T>((object)value);
        public static bool IsDefined<T>(uint value) where T : Enum => IsDefined<T>((object)value);
        public static bool IsDefined<T>(long value) where T : Enum => IsDefined<T>((object)value);
        public static bool IsDefined<T>(ulong value) where T : Enum => IsDefined<T>((object)value);
        public static bool IsDefined<T>(string value) where T : Enum => IsDefined<T>((object)value);

        public static T GetMinValue<T>() where T : Enum => GetValues<T>().Min();
        public static T GetMaxValue<T>() where T : Enum => GetValues<T>().Max();
    }
}

public class NotAnEnumException : Exception
{
    private Type _type;
    public Type Type => _type;

    public NotAnEnumException(Type type) : base($"The given type isn't an enum ({type.FullName} isn't an Enum)")
    {
        _type = type;
    }
    public NotAnEnumException(Type type, Exception innerException) : base($"The given type isn't an enum ({type.FullName} isn't an Enum)", innerException)
    {
        _type = type;
    }
}