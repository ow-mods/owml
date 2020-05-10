using System;
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

        public static PropertyInfo GetAnyProperty(this Type type, string name)
        {
            return type.GetProperty(name, Flags) ??
                   type.BaseType?.GetProperty(name, Flags) ??
                   type.BaseType?.BaseType?.GetProperty(name, Flags);
        }

        public static FieldInfo GetAnyField(this Type type, string name)
        {
            return type.GetField(name, Flags) ??
                   type.BaseType?.GetField(name, Flags) ??
                   type.BaseType?.BaseType?.GetField(name, Flags);
        }

        public static T GetValue<T>(this object obj, string name)
        {
            var type = obj.GetType();
            var field = type.GetAnyField(name);
            if (field != null)
            {
                return (T)field.GetValue(obj);
            }
            var property = type.GetAnyProperty(name);
            if (property != null)
            {
                return (T)property.GetValue(obj, null);
            }
            return default;
        }

        public static void SetValue(this object obj, string name, object value)
        {
            var type = obj.GetType();
            var field = type.GetAnyField(name);
            if (field != null)
            {
                field.SetValue(obj, value);
                return;
            }
            var property = type.GetAnyProperty(name);
            property?.SetValue(obj, value, null);
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