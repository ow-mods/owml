﻿using HarmonyLib;
using OWML.Common;
using OWML.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("OWML.ModLoader")]

namespace OWML.Utils
{
    public static partial class EnumUtils
    {
        /// <summary>
        /// Any enum with flags uses numbers that are a power of two. Some other enums also use it because mobius.
        /// </summary>
        private static readonly HashSet<Type> _powerOfTwoTypes = new HashSet<Type>
        {
            typeof(DreamLanternType),
            typeof(Detector.Name),
            typeof(GroupControlFlags),
            typeof(InputConsts.InputType),
            typeof(InputMode),
            typeof(ItemType),
            typeof(LightSourceType),
            typeof(NotificationTarget),
            typeof(ProxyShadowCascade.Flags),
            typeof(StartupPopups),
            typeof(ToolMode),
            typeof(WarpCoreType),
            typeof(Credits.Platform),
            typeof(Credits.CreditsType),
            typeof(DynamicOccupant),
            typeof(GhostNode.NodeLayer),
            typeof(HazardVolume.HazardType),
            typeof(InputUtil.XInputButton),
            typeof(InputUtil.ScePadButton),
            typeof(Shape.Layer),
            typeof(SignalFrequency),
            typeof(TitleCodeInputManager.CommandSequenceIds)
        };

        internal static void Initialize(IModConsole console, IHarmonyHelper _harmonyHelper)
        {
            console.WriteLine("Initializing enum creator");
            try
            {
                _harmonyHelper.Transpile(AccessTools.Method(Type.GetType("System.Enum"), "GetCachedValuesAndNames"), typeof(EnumInfoPatch), nameof(EnumInfoPatch.Transpiler));
            }
            catch (Exception ex)
            {
                console.WriteLine($"Exception while patching System.Enum.GetCachedValuesAndNames: {ex}", MessageType.Error);
            }
        }

        private static class EnumInfoPatch
        {
            public static void FixEnum(object type, ref ulong[] oldValues, ref string[] oldNames)
            {
                var enumType = type as Type;
                if (TryGetRawPatch(enumType, out var patch))
                {
                    var pairs = patch.GetPairs();

                    List<ulong> newValues = new List<ulong>(oldValues);
                    List<string> newNames = new List<string>(oldNames);

                    foreach (var pair in pairs)
                    {
                        newValues.Add(pair.Key);
                        newNames.Add(pair.Value);
                    }

                    oldValues = newValues.ToArray();
                    oldNames = newNames.ToArray();

                    Array.Sort(oldValues, oldNames, Comparer<ulong>.Default);
                }
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                using (var enumerator = instructions.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var v = enumerator.Current;
                        if (v.operand is MethodInfo me && me.Name == "Sort")
                        {
                            yield return v;
                            enumerator.MoveNext();
                            v = enumerator.Current;
                            var labels = v.labels;
                            v.labels = new List<Label>();
                            yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = labels };
                            yield return new CodeInstruction(OpCodes.Ldloca, 1);
                            yield return new CodeInstruction(OpCodes.Ldloca, 2);
                            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EnumInfoPatch), "FixEnum"));
                            yield return v;
                        }
                        else
                        {
                            yield return v;
                        }
                    }
                }
            }
        }


        private static Dictionary<Type, EnumPatch> patches = new Dictionary<Type, EnumPatch>();


        private static FieldInfo cache = AccessTools.Field(AccessTools.TypeByName("System.RuntimeType"), "GenericCache");
        private static void ClearEnumCache(Type enumType) => cache.SetValue(enumType, null);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name) where T : Enum => (T)Create(typeof(T), name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name, T value) where T : Enum => Create<T>(value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name, short value) where T : Enum => Create<T>(value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name, ushort value) where T : Enum => Create<T>(value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name, int value) where T : Enum => Create<T>(value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name, uint value) where T : Enum => Create<T>(value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name, long value) where T : Enum => Create<T>(value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name, ulong value) where T : Enum => Create<T>(value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name, byte value) where T : Enum => Create<T>(value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(string name, sbyte value) where T : Enum => Create<T>(value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(short value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(ushort value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(int value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(uint value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(long value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(ulong value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(byte value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(sbyte value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static T Create<T>(object value, string name) where T : Enum
        {
            Create(typeof(T), value, name);
            return (T)value;
        }

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns>The created enum value</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static object Create(Type enumType, string name)
        {
            var newVal = GetFirstFreeValue(enumType);
            Create(enumType, GetFirstFreeValue(enumType), name);
            return newVal;
        }

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <param name="value">Value of the enum</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static void Create(Type enumType, string name, object value) => Create(enumType, value, name);

        /// <summary>
        /// Creates an actual enum value associated with a name
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        /// <exception cref="Exception">The enum already has a value with the same name</exception>
        public static void Create(Type enumType, object value, string name)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            if (AlreadyHasName(enumType, name) || IsDefined(enumType, name)) throw new Exception($"The enum ({enumType.FullName}) already has a value with the name \"{name}\"");

            if (!TryGetRawPatch(enumType, out var patch))
            {
                patch = new EnumPatch();
                patches.Add(enumType, patch);
            }

            patch.AddValue(value.ToFriendlyValue(), name);

            // Clear enum cache
            ClearEnumCache(enumType);
        }

        internal static ulong ToFriendlyValue<T>(this T value) where T : Enum => (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);
        internal static ulong ToFriendlyValue(this object value) => (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);

        /// <summary>
        /// Removes a custom enum value from being associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        public static void Remove<T>(string name) where T : Enum => Remove(typeof(T), name);

        /// <summary>
        /// Removes a custom enum value from being associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">The enum value to remove</param>
        public static void Remove<T>(T value) where T : Enum => Remove(typeof(T), value);

        /// <summary>
        /// Removes a custom enum value from being associated with a name
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">The enum value to remove</param>
        public static void Remove<T>(object value) where T : Enum => Remove(typeof(T), value);

        /// <summary>
        /// Removes a custom enum value from being associated with a name
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static void Remove(Type enumType, string name)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            if (TryGetRawPatch(enumType, out EnumPatch patch) && patch.HasName(name))
            {
                patch.RemoveValue(name);

                // Clear enum cache
                ClearEnumCache(enumType);
            }
        }

        /// <summary>
        /// Removes a custom enum value from being associated with a name
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value of the enum</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static void Remove(Type enumType, object value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            ulong uvalue = value.ToFriendlyValue();
            if (TryGetRawPatch(enumType, out EnumPatch patch) && patch.HasValue(uvalue))
            {
                patch.RemoveValue(uvalue);

                // Clear enum cache
                ClearEnumCache(enumType);
            }
        }

        /// <summary>
        /// Check if it is a custom enum value
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        public static bool IsDynamic<T>(string name) where T : Enum => IsDynamic(typeof(T), name);

        /// <summary>
        /// Check if it is a custom enum value
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">The enum value to check</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        public static bool IsDynamic<T>(this T value) where T : Enum => IsDynamic(typeof(T), value);

        /// <summary>
        /// Check if it is a custom enum value
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">The enum value to check</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        public static bool IsDynamic<T>(object value) where T : Enum => IsDynamic(typeof(T), value);

        /// <summary>
        /// Check if it is a custom enum value
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDynamic(Type enumType, string name)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);

            return TryGetRawPatch(enumType, out EnumPatch patch) && patch.HasName(name);
        }

        /// <summary>
        /// Check if it is a custom enum value
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value of the enum</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsDynamic(Type enumType, object value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);

            ulong uvalue = value.ToFriendlyValue();
            return TryGetRawPatch(enumType, out EnumPatch patch) && patch.HasValue(uvalue);
        }

        /// <summary>
        /// Check if it is <b>not</b> a custom enum value
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">Name of the enum value</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        public static bool IsStatic<T>(string name) where T : Enum => IsStatic(typeof(T), name);

        /// <summary>
        /// Check if it is <b>not</b> a custom enum value
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">The enum value to check</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        public static bool IsStatic<T>(this T value) where T : Enum => IsStatic(typeof(T), value);

        /// <summary>
        /// Check if it is <b>not</b> a custom enum value
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="value">The enum value to check</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        public static bool IsStatic<T>(object value) where T : Enum => IsStatic(typeof(T), value);

        /// <summary>
        /// Check if it is <b>not</b> a custom enum value
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="name">Name of the enum value</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsStatic(Type enumType, string name)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);

            return !IsDynamic(enumType, name);
        }

        /// <summary>
        /// Check if it is <b>not</b> a custom enum value
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <param name="value">Value of the enum</param>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsStatic(Type enumType, object value)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);

            return !IsDynamic(enumType, value);
        }

        private static bool TryAsNumber(this object value, Type type, out object result)
        {
            if (type.IsSubclassOf(typeof(IConvertible)))
                throw new ArgumentException("The type must inherit the IConvertible interface", "type");
            result = null;
            if (type.IsInstanceOfType(value))
            {
                result = value;
                return true;
            }
            if (value is IConvertible)
            {
                if (type.IsEnum)
                {
                    result = Enum.ToObject(type, value);
                    return true;
                }
                var format = NumberFormatInfo.CurrentInfo;
                result = (value as IConvertible).ToType(type, format);
                return true;
            }
            return false;
        }

        private static bool IsPowerOfTwo(this ulong x)
        {
            return x > 0 && (x & (x - 1)) == 0;
        }

        /// <summary>
        /// Does this enum use power of twos?
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns><see langword="true"/> if it does, <see langword="false"/> if not.</returns>
        public static bool IsPowerOfTwoEnum<T>() where T : Enum => _powerOfTwoTypes.Contains(typeof(T)) || typeof(T).IsDefined(typeof(FlagsAttribute), false);

        /// <summary>
        /// Does this enum use power of twos?
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns><see langword="true"/> if it does, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        public static bool IsPowerOfTwoEnum(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return _powerOfTwoTypes.Contains(enumType) || enumType.IsDefined(typeof(FlagsAttribute), false);
        }

        /// <summary>
        /// Get first undefined value in an enum
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <returns>The first undefined enum value</returns>
        /// <exception cref="Exception">No unused values in the enum</exception>
        public static T GetFirstFreeValue<T>() where T : Enum => (T)GetFirstFreeValue(typeof(T));

        /// <summary>
        /// Get first undefined value in an enum
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>The first undefined enum value</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <see langword="null"/></exception>
        /// <exception cref="NotAnEnumException"><paramref name="enumType"/> is not an enum</exception>
        /// <exception cref="Exception">No unused values in the enum</exception>
        public static object GetFirstFreeValue(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);

            var vals = Enum.GetValues(enumType);
            var usesPowerOfTwo = IsPowerOfTwoEnum(enumType);
            long l = 0;
            for (ulong i = 0; i <= ulong.MaxValue; i++)
            {
                if (usesPowerOfTwo && !IsPowerOfTwo(i)) continue;
                if (!i.TryAsNumber(enumType, out var v))
                    break;
                for (; l < vals.LongLength; l++)
                    if (vals.GetValue(l).Equals(v))
                        goto skip;
                return v;
            skip:;
            }
            if (usesPowerOfTwo) goto no_negatives;
            for (long i = -1; i >= long.MinValue; i--)
            {
                if (!i.TryAsNumber(enumType, out var v))
                    break;
                for (; l < vals.LongLength; l++)
                    if (vals.GetValue(l).Equals(v))
                        goto skip;
                return v;
            skip:;
            }
            no_negatives:
            throw new Exception((usesPowerOfTwo ? "No unused values in power of two enum " : "No unused values in enum ") + enumType.FullName);
        }

        private static bool TryGetRawPatch<T>(out EnumPatch patch) where T : Enum => TryGetRawPatch(typeof(T), out patch);

        private static bool TryGetRawPatch(Type enumType, out EnumPatch patch)
        {
            return patches.TryGetValue(enumType, out patch);
        }

        private static bool AlreadyHasName(Type enumType, string name)
        {
            if (TryGetRawPatch(enumType, out EnumPatch patch))
                return patch.HasName(name);
            return false;
        }

        private class EnumPatch
        {
            private Dictionary<ulong, List<string>> values = new Dictionary<ulong, List<string>>();

            public List<KeyValuePair<ulong, string>> GetPairs()
            {
                List<KeyValuePair<ulong, string>> pairs = new List<KeyValuePair<ulong, string>>();
                foreach (KeyValuePair<ulong, List<string>> pair in values)
                {
                    foreach (string value in pair.Value)
                        pairs.Add(new KeyValuePair<ulong, string>(pair.Key, value));
                }
                return pairs;
            }

            public bool HasValue(ulong value)
            {
                return values.Keys.Contains(value);
            }

            public bool HasName(string name)
            {
                foreach (string enumName in this.values.Values.SelectMany(l => l))
                {
                    if (name.Equals(enumName))
                        return true;
                }
                return false;
            }

            public void AddValue(ulong enumValue, string name)
            {
                if (values.ContainsKey(enumValue))
                    values[enumValue].Add(name);
                else
                    values.Add(enumValue, new List<string> { name });
            }

            public void RemoveValue(ulong enumValue) => values.Remove(enumValue);

            public void RemoveValue(string name)
            {
                if (string.IsNullOrEmpty(name)) return;
                foreach (var pair in values) pair.Value.Remove(name);
            }
        }

        internal static void RegisterAllEnums(Module module)
        {
            foreach (var type in module.GetTypes())
            {
                if (type.IsDefined(typeof(EnumHolderAttribute), true))
                {
                    foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (!field.FieldType.IsEnum) continue;

                        if (Convert.ToInt64(field.GetValue(null)) == 0)
                        {
                            field.SetValue(null, Create(field.FieldType, field.Name));
                        }
                        else
                            Create(field.FieldType, field.GetValue(null), field.Name);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Add this attribute to a class, and any static enum fields will have an enum value created with the name of the field.
    /// </summary>
    public class EnumHolderAttribute : Attribute { }
}