using System;
using System.Collections.Generic;
using System.Linq;

namespace OWML.Utils
{
    /// <summary>
    /// An utility class to help with Enums
    /// </summary>
    public static partial class EnumUtils
    {
        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed enum on success, null on failure.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
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

        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to parse</param>
        /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
        /// <returns>The parsed enum on success, null on failure.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
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

        /// <summary>
        /// Converts a number to an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object FromObject(Type enumType, object value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Converts a <see cref="sbyte"/> to an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object FromObject(Type enumType, sbyte value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Converts a <see cref="byte"/> to an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object FromObject(Type enumType, byte value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Converts a <see cref="short"/> to an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object FromObject(Type enumType, short value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Converts an <see cref="ushort"/> to an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object FromObject(Type enumType, ushort value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Converts an <see cref="int"/> to an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object FromObject(Type enumType, int value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Converts an <see cref="uint"/> to an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object FromObject(Type enumType, uint value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Converts a <see cref="long"/> to an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object FromObject(Type enumType, long value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Converts an <see cref="ulong"/> to an enum.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object FromObject(Type enumType, ulong value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Gets all names in an enum
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>An array of the names in the enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static string[] GetNames(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.GetNames(enumType);
        }

        /// <summary>
        /// Gets all values in an enum
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>An array of the values in the enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object[] GetValues(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.GetValues(enumType).Cast<object>().ToArray();
        }

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
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

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDefined(Type enumType, sbyte value) => IsDefined(enumType, (object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDefined(Type enumType, byte value) => IsDefined(enumType, (object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDefined(Type enumType, short value) => IsDefined(enumType, (object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDefined(Type enumType, ushort value) => IsDefined(enumType, (object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDefined(Type enumType, int value) => IsDefined(enumType, (object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDefined(Type enumType, uint value) => IsDefined(enumType, (object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDefined(Type enumType, long value) => IsDefined(enumType, (object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDefined(Type enumType, ulong value) => IsDefined(enumType, (object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDefined(Type enumType, string value) => IsDefined(enumType, (object)value);

        /// <summary>
        /// Gets the minimum value in the enum
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>the minimum value in the enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object GetMinValue(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return GetValues(enumType).Min();
        }

        /// <summary>
        /// Gets the maximum value in the enum
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>the maximum value in the enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object GetMaxValue(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return GetValues(enumType).Max();
        }

        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to parse</param>
        /// <param name="errorReturn">What to return if the parse fails.</param>
        /// <returns>The parsed enum on success, <paramref name="errorReturn"/> on failure.</returns>
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

        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to parse</param>
        /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
        /// <param name="errorReturn">What to return if the parse fails.</param>
        /// <returns>The parsed enum on success, <paramref name="errorReturn"/> on failure.</returns>
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


        /// <summary>
        /// Converts a number to an enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        public static T FromObject<T>(object value) where T : Enum => (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// Converts a <see cref="sbyte"/> to an enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        public static T FromObject<T>(sbyte value) where T : Enum => (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// Converts a <see cref="byte"/> to an enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        public static T FromObject<T>(byte value) where T : Enum => (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// Converts a <see cref="short"/> to an enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        public static T FromObject<T>(short value) where T : Enum => (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// Converts an <see cref="ushort"/> to an enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        public static T FromObject<T>(ushort value) where T : Enum => (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// Converts an <see cref="int"/> to an enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        public static T FromObject<T>(int value) where T : Enum => (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// Converts an <see cref="uint"/> to an enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        public static T FromObject<T>(uint value) where T : Enum => (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// Converts a <see cref="long"/> to an enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        public static T FromObject<T>(long value) where T : Enum => (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// Converts an <see cref="ulong"/> to an enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The number as an enum</returns>
        public static T FromObject<T>(ulong value) where T : Enum => (T)Enum.ToObject(typeof(T), value);


        /// <summary>
        /// Gets all names in an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The list of names in the enum</returns>
        public static string[] GetNames<T>() where T : Enum => Enum.GetNames(typeof(T));

        /// <summary>
        /// Gets all enum values in an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The list of all values in the enum</returns>
        public static T[] GetValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().ToArray();

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
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

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(sbyte value) where T : Enum => IsDefined<T>((object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(byte value) where T : Enum => IsDefined<T>((object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(short value) where T : Enum => IsDefined<T>((object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(ushort value) where T : Enum => IsDefined<T>((object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(int value) where T : Enum => IsDefined<T>((object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(uint value) where T : Enum => IsDefined<T>((object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(long value) where T : Enum => IsDefined<T>((object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(ulong value) where T : Enum => IsDefined<T>((object)value);

        /// <summary>
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(string value) where T : Enum => IsDefined<T>((object)value);

        /// <summary>
        /// Gets the minimum value in the enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>the minimum value in the enum</returns>
        public static T GetMinValue<T>() where T : Enum => GetValues<T>().Min();

        /// <summary>
        /// Gets the maximum value in the enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>the maximum value in the enum</returns>
        public static T GetMaxValue<T>() where T : Enum => GetValues<T>().Max();
    }
}

/// <summary>
/// The exception that is thrown when an enum type is needed but the given type is not an enum.
/// </summary>
public class NotAnEnumException : Exception
{
    private Type _type;

    /// <summary>
    /// The type that is not an enum
    /// </summary>
    public Type Type => _type;

    /// <summary>
    /// Initializes a new instance of the NotAnEnumException class with a type that is not an enum.
    /// </summary>
    /// <param name="type">The type that is not an enum</param>
    public NotAnEnumException(Type type) : base($"The given type isn't an enum ({type.FullName} isn't an Enum)")
    {
        _type = type;
    }

    /// <summary>
    /// Initializes a new instance of the NotAnEnumException class with a type that is not an enum and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="type">The type that is not an enum</param>
    /// <param name="innerException">The exception caused the current exception</param>
    public NotAnEnumException(Type type, Exception innerException) : base($"The given type isn't an enum ({type.FullName} isn't an Enum)", innerException)
    {
        _type = type;
    }
}