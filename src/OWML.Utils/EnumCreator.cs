using HarmonyLib;
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
        private static readonly HashSet<Type> _flagsTypes = new HashSet<Type>
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

        public static T Create<T>(string name) where T : Enum => (T)Create(typeof(T), name);
        public static void Create<T>(string name, T value) where T : Enum => Create<T>(value, name);
        public static T Create<T>(string name, short value) where T : Enum => Create<T>(value, name);
        public static T Create<T>(string name, ushort value) where T : Enum => Create<T>(value, name);
        public static T Create<T>(string name, int value) where T : Enum => Create<T>(value, name);
        public static T Create<T>(string name, uint value) where T : Enum => Create<T>(value, name);
        public static T Create<T>(string name, long value) where T : Enum => Create<T>(value, name);
        public static T Create<T>(string name, ulong value) where T : Enum => Create<T>(value, name);
        public static T Create<T>(string name, byte value) where T : Enum => Create<T>(value, name);
        public static T Create<T>(string name, sbyte value) where T : Enum => Create<T>(value, name);
        public static T Create<T>(short value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);
        public static T Create<T>(ushort value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);
        public static T Create<T>(int value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);
        public static T Create<T>(uint value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);
        public static T Create<T>(long value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);
        public static T Create<T>(ulong value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);
        public static T Create<T>(byte value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);
        public static T Create<T>(sbyte value, string name) where T : Enum => Create<T>(Enum.ToObject(typeof(T), value), name);

        public static T Create<T>(object value, string name) where T : Enum
        {
            Create(typeof(T), value, name);
            return (T)value;
        }

        public static object Create(Type enumType, string name)
        {
            var newVal = GetFirstFreeValue(enumType);
            Create(enumType, GetFirstFreeValue(enumType), name);
            return newVal;
        }

        public static void Create(Type enumType, string name, object value) => Create(enumType, value, name);

        public static void Create(Type enumType, object value, string name)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            if (AlreadyHasName(enumType, name) || IsDefined(enumType, name)) throw new Exception($"The enum ({enumType.FullName}) already has a value with the name \"{name}\"");

            if (!patches.TryGetValue(enumType, out var patch))
            {
                patch = new EnumPatch();
                patches.Add(enumType, patch);
            }

            patch.AddValue((ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture), name);

            // Clear enum cache
            ClearEnumCache(enumType);
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

        public static bool IsFlagsEnum<T>() where T : Enum => _flagsTypes.Contains(typeof(T)) || typeof(T).IsDefined(typeof(FlagsAttribute), false);
        public static bool IsFlagsEnum(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);
            return _flagsTypes.Contains(enumType) || enumType.IsDefined(typeof(FlagsAttribute), false);
        }

        public static T GetFirstFreeValue<T>() where T : Enum => (T)GetFirstFreeValue(typeof(T));

        public static object GetFirstFreeValue(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new NotAnEnumException(enumType);

            var vals = Enum.GetValues(enumType);
            var flags = IsFlagsEnum(enumType);
            long l = 0;
            for (ulong i = 0; i <= ulong.MaxValue; i++)
            {
                if (flags && !IsPowerOfTwo(i)) continue;
                if (!i.TryAsNumber(enumType, out var v))
                    break;
                for (; l < vals.LongLength; l++)
                    if (vals.GetValue(l).Equals(v))
                        goto skip;
                return v;
            skip:;
            }
            if (flags) goto no_negatives;
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
            throw new Exception((flags ? "No unused values in flags enum " : "No unused values in enum ") + enumType.FullName);
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
        }
    }
}