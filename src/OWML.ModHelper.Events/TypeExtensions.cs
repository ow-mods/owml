using System;
using System.Reflection;

namespace OWML.ModHelper.Events
{
    [Obsolete("Use OWML.Utils.TypeExtensions instead.")]
    public static class TypeExtensions
    {
        [Obsolete("Use OWML.Utils.TypeExtensions.GetAnyMethod instead.")]
        public static MethodInfo GetAnyMethod(this Type type, string name) => 
            Utils.TypeExtensions.GetAnyMethod(type, name);

        [Obsolete("Use OWML.Utils.TypeExtensions.GetAnyMember instead.")]
        public static PropertyInfo GetAnyProperty(this Type type, string name) => 
            Utils.TypeExtensions.GetAnyMember(type, name) as PropertyInfo;

        [Obsolete("Use OWML.Utils.TypeExtensions.GetAnyMember instead.")]
        public static FieldInfo GetAnyField(this Type type, string name) =>
            Utils.TypeExtensions.GetAnyMember(type, name) as FieldInfo;

        [Obsolete("Use OWML.Utils.TypeExtensions.GetValue instead.")]
        public static T GetValue<T>(this object obj, string name) => 
            Utils.TypeExtensions.GetValue<T>(obj, name);

        [Obsolete("Use OWML.Utils.TypeExtensions.SetValue instead.")]
        public static void SetValue(this object obj, string name, object value) => 
            Utils.TypeExtensions.SetValue(obj, name, value);

        [Obsolete("Use OWML.Utils.TypeExtensions.Invoke instead.")]
        public static void Invoke(this object obj, string name, params object[] parameters) => 
            Utils.TypeExtensions.Invoke(obj, name, parameters);

        [Obsolete("Use OWML.Utils.TypeExtensions.Invoke instead.")]
        public static T Invoke<T>(this object obj, string name, params object[] parameters) => 
            Utils.TypeExtensions.Invoke<T>(obj, name, parameters);
    }
}
