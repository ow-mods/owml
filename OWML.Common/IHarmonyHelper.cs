using System;

namespace OWML.Common
{
    public interface IHarmonyHelper
    {
        void AddPrefix<T>(string methodName, Type patchType, string patchMethodName);
        void AddPrefix<T>(string methodName, Type patchType, string patchMethodName, Type[] parameters);
        void AddPostfix<T>(string methodName, Type patchType, string patchMethodName);
        void AddPostfix<T>(string methodName, Type patchType, string patchMethodName, Type[] parameters);
        void EmptyMethod<T>(string methodName);
        void EmptyMethod<T>(string methodName, Type[] parameters);
        void Transpile<T>(string methodName, Type patchType, string patchMethodName);
        void Transpile<T>(string methodName, Type patchType, string patchMethodName, Type[] parameters);
    }
}