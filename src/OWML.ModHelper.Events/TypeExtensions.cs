using System;
using System.Linq;
using System.Reflection;

namespace OWML.ModHelper.Events
{
    public static class TypeExtensions
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        public static MethodInfo GetAnyMethod(this Type type, string name)
        {
            return type.GetMethod(name, Flags) ??
                   type.BaseType?.GetMethod(name, Flags) ??
                   type.BaseType?.BaseType?.GetMethod(name, Flags);
        }

        public static MemberInfo GetAnyMember(this Type type, string name)
        {
            return type.GetMember(name, Flags).FirstOrDefault() ??
                   type.BaseType?.GetMember(name, Flags).FirstOrDefault() ??
                   type.BaseType?.BaseType?.GetMember(name, Flags).FirstOrDefault();
        }

        public static T GetValue<T>(this object obj, string name)
        {
            var member = obj.GetType().GetAnyMember(name);
            switch (member)
            {
                case FieldInfo field:
                    return (T)field.GetValue(obj);
                case PropertyInfo property:
                    return (T)property.GetValue(obj, null);
                default:
                    return default;
            }
        }

        public static void SetValue(this object obj, string name, object value)
        {
            var member = obj.GetType().GetAnyMember(name);
            switch (member)
            {
                case FieldInfo field:
                    field.SetValue(obj, value);
                    break;
                case PropertyInfo property:
                    property.SetValue(obj, value, null);
                    break;
            }
        }

        public static void Invoke(this object obj, string name, params object[] parameters)
        {
            Invoke<object>(obj, name, parameters);
        }

        public static T Invoke<T>(this object obj, string name, params object[] parameters)
        {
            var type = obj.GetType();
            var method = type.GetAnyMethod(name);
            return (T)method?.Invoke(obj, parameters);
        }
    }
}