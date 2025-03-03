using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace OWML.Utils
{
    /// <summary>
    /// An utility class to help with Enums
    /// </summary>
    public static partial class EnumUtils
    {
        private static readonly Random Rng = new Random();

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
        /// Parses an enum in a easier way
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to parse</param>
        /// <param name="result">The parsed enum if successful.</param>
        /// <returns><see langword="true"/> on success, <see langword="false"/> on failure.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool TryParse(Type enumType, string value, out object result)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            try
            {
                result = Enum.Parse(enumType, value);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to parse</param>
        /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
        /// <param name="result">The parsed enum if successful.</param>
        /// <returns><see langword="true"/> on success, <see langword="false"/> on failure.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool TryParse(Type enumType, string value, bool ignoreCase, out object result)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            try
            {
                result = Enum.Parse(enumType, value, ignoreCase);
                return true;
            }
            catch
            {
                result = null;
                return false;
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
        /// Gets the underlying type of an enum type.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>The underlying type of <paramref name="enumType"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static Type GetUnderlyingType(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.GetUnderlyingType(enumType);
        }

        /// <summary>
        /// Get the name of an enum value
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value to get the name of</param>
        /// <returns>The name of the enum value</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static string GetName(Type enumType, object value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.GetName(enumType, value);
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
        /// Gets all names in an enum, skipping the first (usually default,none) value
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>An array of the names (excluding the first value) in the enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static string[] GetNamesWithoutFirst(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.GetNames(enumType).Skip(1).ToArray();
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
        /// Gets all enum values in an enum, skipping the first (usually default,none) value
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>An array of the values (excluding the first value) in the enum</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static object[] GetValuesWithoutFirst(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return System.Enum.GetValues(enumType).Cast<object>().Skip(1).ToArray();
        }

        /// <summary>
        /// Counts the number of enums values contained in a given enum type.
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>The number of enum values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static int Count(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return Enum.GetValues(enumType).Length;
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
        /// Gets a random value from an enum
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>A randomly selected enum value from the given enum type</returns>
        public static object GetRandom(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            var values = Enum.GetValues(enumType);
            var item = Rng.Next(0, values.Length);
            return values.GetValue(item);
        }

        /// <summary>
        /// Gets a random value from an enum with exclusions
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="excluded">Enums to exclude from the randomization</param>
        /// <returns>A randomly selected enum value from the given enum type</returns>
        public static object GetRandom(Type enumType, params object[] excluded)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            if (excluded == null) throw new ArgumentNullException("excluded");
            var values = Enum.GetValues(enumType).Cast<object>().Where(v => !excluded.Contains(v)).ToArray();
            var item = Rng.Next(0, values.Length);
            return values.GetValue(item);
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
        /// Parses an enum in a easier way
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to parse</param>
        /// <param name="result">The parsed enum if successful.</param>
        /// <returns><see langword="true"/> on success, <see langword="false"/> on failure.</returns>
        public static bool TryParse<T>(string value, out T result) where T : Enum
        {
            try
            {
                result = (T)Enum.Parse(typeof(T), value);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Parses an enum in a easier way
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to parse</param>
        /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
        /// <param name="result">The parsed enum if successful.</param>
        /// <returns><see langword="true"/> on success, <see langword="false"/> on failure.</returns>
        public static bool TryParse<T>(string value, bool ignoreCase, out T result) where T : Enum
        {
            try
            {
                result = (T)Enum.Parse(typeof(T), value, ignoreCase);
                return true;
            }
            catch
            {
                result = default;
                return false;
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
        /// Gets the underlying type of an enum type.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The underlying type of <typeparamref name="T"/></returns>
        public static Type GetUnderlyingType<T>() where T : Enum => Enum.GetUnderlyingType(typeof(T));

        /// <summary>
        /// Get the name of an enum value
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to get the name of</param>
        /// <returns>The name of the enum value</returns>
        public static string GetName<T>(this T value) where T : Enum => Enum.GetName(typeof(T), value);

        /// <summary>
        /// Gets all names in an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The list of names in the enum</returns>
        public static string[] GetNames<T>() where T : Enum => Enum.GetNames(typeof(T));

        /// <summary>
        /// Gets all names in an enum, skipping the first (usually default,none) value
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The list of all names (excluding the first value) in the enum</returns>
        public static string[] GetNamesWithoutFirst<T>() where T : Enum => Enum.GetNames(typeof(T)).Skip(1).ToArray();

        /// <summary>
        /// Gets all names in an enum with exclusions
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="excluded">Enums to exclude from the randomization</param>
        /// <returns>The list of names in the enum</returns>
        public static string[] GetNames<T>(params T[] excluded) where T : Enum => GetValues<T>(excluded).Select(GetName).ToArray();

        /// <summary>
        /// Gets all enum values in an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The list of all values in the enum</returns>
        public static T[] GetValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().ToArray();

        /// <summary>
        /// Gets all enum values in an enum, skipping the first (usually default,none) value
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The list of all values (excluding the first value) in the enum</returns>
        public static T[] GetValuesWithoutFirst<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Skip(1).ToArray();

        /// <summary>
        /// Gets all enum values in an enum with exclusions
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="excluded">Enums to exclude from the randomization</param>
        /// <returns>The list of all values in the enum</returns>
        public static T[] GetValues<T>(params T[] excluded) where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Where(v => !excluded.Contains(v)).ToArray();

        /// <summary>
        /// Gets all dynamic (custom) enum values in an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The list of all dynamic values in the enum</returns>
        public static T[] GetDynamicValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Where(IsDynamic).ToArray();

        /// <summary>
        /// Gets all static (non-custom) enum values in an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The list of all static values in the enum</returns>
        public static T[] GetStaticValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Where(IsStatic).ToArray();

        /// <summary>
        /// Counts the number of enums values contained in a given enum type.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The number of enum values.</returns>
        public static int Count<T>() where T : Enum => Enum.GetValues(typeof(T)).Length;

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
        /// Checks if an enum is defined.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns><see langword="true"/> if defined, <see langword="false"/> if not.</returns>
        public static bool IsDefined<T>(this T value) where T : Enum => IsDefined<T>((object)value);

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

        /// <summary>
        /// Converts the enum <paramref name="value"/> to its string representation according to the given format.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <param name="format">The output format to use.</param>
        /// <returns>The underlying type of <typeparamref name="T"/></returns>
        public static string Format<T>(this T value, string format) where T : Enum => Enum.Format(typeof(T), value, format);

        /// <summary>
        /// Gets a random value from an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>A randomly selected enum value from the given enum type</returns>
        public static T GetRandom<T>() where T : Enum
        {
            var values = EnumUtils.GetValues<T>();
            var item = Rng.Next(0, values.Length);
            return (T)values.GetValue(item);
        }

        /// <summary>
        /// Gets a random value from an enum with exclusions
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="excluded">Enums to exclude from the randomization</param>
        /// <returns>A randomly selected enum value from the given enum type</returns>
        public static T GetRandom<T>(params T[] excluded) where T : Enum
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>().Where(v => !excluded.Contains(v)).ToArray();
            var item = Rng.Next(0, values.Length);
            return (T)values.GetValue(item);
        }

        /// <summary>
        /// Gets a random name from an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>A randomly selected enum name from the given enum type</returns>
        public static string GetRandomName<T>() where T : Enum
        {
            var names = EnumUtils.GetNames<T>();
            var item = Rng.Next(0, names.Length);
            return (string)names.GetValue(item);
        }

        /// <summary>
        /// Gets a random name from an enum with exclusions
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="excluded">Enums to exclude from the randomization</param>
        /// <returns>A randomly selected enum name from the given enum type</returns>
        public static string GetRandomName<T>(params T[] excluded) where T : Enum
        {
            var names = Enum.GetNames(typeof(T)).Cast<T>().Where(v => !excluded.Contains(v)).ToArray();
            var item = Rng.Next(0, names.Length);
            return (string)names.GetValue(item);
        }

        /// <summary>
        /// Returns all enums with their descriptions in a dictionary.
        /// <see cref="DescriptionAttribute"/> needs to be applied on enum values to set a description text.
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>Dictionary of enum-to-description mappings.</returns>
        public static IDictionary<T, string> GetDescriptions<T>() where T : Enum
        {
            var type = typeof(T);
            var dictionary = new Dictionary<T, string>();

            foreach (var key in GetValues<T>())
            {
                var field = type.GetField($"{key}");
                string description = null;
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    description = attribute.Description;
                }

                dictionary.Add(key, description);
            }

            return dictionary;
        }

        /// <summary>
        /// Throws a <see cref="NotAnEnumException"/> if <paramref name="type"/> is not an enum.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="type"/> is not an enum</exception>
        public static void ThrowIfNotEnum(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (!type.IsEnum) throw new NotAnEnumException(type);
        }

        /// <summary>
        /// Throws a <see cref="NotAnEnumException"/> if <typeparamref name="T"/> is not an enum.
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <exception cref="NotAnEnumException"><typeparamref name="T"/> is not an enum</exception>
        public static void ThrowIfNotEnum<T>()
        {
            if (!typeof(T).IsEnum) throw new NotAnEnumException(typeof(T));
        }

        /// <summary>
        /// Casts an Enum to a specific type
        /// </summary>
        public static T EnumCast<T>(this Enum value) where T : Enum
        {
            if (value.GetType() != typeof(T))
                throw new InvalidCastException("Enums are not of the same type");
            return (T)(object)value;
        }

        /// <summary>
        /// Casts an Enum to a specific type
        /// </summary>
        public static IEnumerable<T> EnumCast<T>(this IEnumerable<Enum> values) where T : Enum => values.Select(e => e.EnumCast<T>());

        /// <inheritdoc cref="Enum.HasFlag(Enum)"/>
        public static bool HasFlag<T>(T flags, T flag) where T : Enum
            => flags.HasFlag(flag);

        /// <summary>
        /// Sets a flag bit to 0 or 1
        /// </summary>
        public static T SetFlag<T>(this T flags, T flag, bool setBit) where T : Enum
            => setBit ? AddFlag(flags, flag) : RemoveFlag(flags, flag);

        /// <summary>
        /// Adds a flag to an enum
        /// </summary>
        public static T AddFlag<T>(this T flags, T flag) where T : Enum
            => FromObject<T>(flags.ToFriendlyValue() | flag.ToFriendlyValue());

        /// <summary>
        /// Removes a flag from an enum
        /// </summary>
        public static T RemoveFlag<T>(this T flags, T flag) where T : Enum
            => FromObject<T>(flags.ToFriendlyValue() & ~flag.ToFriendlyValue());

        /// <summary>
        /// Toggles a flag in an enum
        /// </summary>
        public static T ToggleFlag<T>(this T flags, T flag) where T : Enum
            => FromObject<T>(flags.ToFriendlyValue() ^ flag.ToFriendlyValue());

        /// <summary>
        /// 1 &lt;&lt; <paramref name="index"/>
        /// </summary>
        public static T GetFlagsValue<T>(int index) where T : Enum
            => FromObject<T>(1 << index);

        /// <summary>
        /// 0
        /// </summary>
        public static T GetNoFlags<T>() where T : Enum
            => FromObject<T>(0);

        /// <summary>
        /// ~0
        /// </summary>
        public static T GetAllFlags<T>() where T : Enum
            => FromObject<T>(~0);

        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="flags">The value to get the flags from.</param>
        /// <returns>All the flags that the value had.</returns>
        /// <exception cref="ArgumentException">When the enum doesn't have the flags attribute.</exception>
        public static T[] GetFlagsValues<T>(this T flags) where T : Enum
        {
            if (!IsPowerOfTwoEnum<T>())
                throw new ArgumentException(string.Format("The type '{0}' must have an attribute '{1}'.", typeof(T), typeof(FlagsAttribute)));

            Type underlyingType = GetUnderlyingType<T>();

            ulong num = flags.ToFriendlyValue();
            var enumNameValues = GetValues<T>().Select(ToFriendlyValue);
            IList<T> selectedFlagsValues = new List<T>();

            foreach (ulong enumNameValue in enumNameValues)
            {
                if ((num & enumNameValue) == enumNameValue && enumNameValue != 0)
                {
                    selectedFlagsValues.Add((T)Convert.ChangeType(enumNameValue, underlyingType, CultureInfo.CurrentCulture));
                }
            }

            if (selectedFlagsValues.Count == 0 && enumNameValues.SingleOrDefault(v => v == 0) != 0)
            {
                selectedFlagsValues.Add(default(T));
            }

            return selectedFlagsValues.ToArray();
        }

        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="values"></param>
        /// <returns>All the flags values combined into one enum value.</returns>
        /// <exception cref="ArgumentException">When the enum doesn't have the flags attribute.</exception>
        public static T CombineFlagsValues<T>(this T[] values) where T : Enum
        {
            if (!IsPowerOfTwoEnum<T>())
                throw new ArgumentException(string.Format("The type '{0}' must have an attribute '{1}'.", typeof(T), typeof(FlagsAttribute)));

            T combined = default(T);
            if (values != null && values.Length > 0)
            {
                foreach (var value in values)
                {
                    combined = combined.AddFlag<T>(value);
                }
            }
            return combined;
        }
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