using System;
using System.Reflection;

namespace OWML.Common
{
    public interface IHarmonyHelper
    {
        void AddPrefix<T>(string methodName, Type patchType, string patchMethodName);
        void AddPrefix(MethodInfo methodInfo, Type patchType, string patchMethodName);
        void AddPostfix<T>(string methodName, Type patchType, string patchMethodName);
        void AddPostfix(MethodInfo methodInfo, Type patchType, string patchMethodName);
        void EmptyMethod<T>(string methodName);
        void EmptyMethod(MethodInfo methodInfo);
        void Transpile<T>(string methodName, Type patchType, string patchMethodName);
        void Transpile(MethodInfo methodInfo, Type patchType, string patchMethodName);
    }
}
