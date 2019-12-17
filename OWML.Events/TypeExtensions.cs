using System;
using System.Reflection;

namespace OWML.Events
{
    public static class TypeExtensions
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        public static MethodInfo GetAnyMethod(this Type type, string name)
        {
            return type.GetMethod(name, Flags);
        }

        public static PropertyInfo GetAnyProperty(this Type type, string name)
        {
            return type.GetProperty(name, Flags);
        }

        public static FieldInfo GetAnyField(this Type type, string name)
        {
            return type.GetField(name, Flags);
        }

    }
}
