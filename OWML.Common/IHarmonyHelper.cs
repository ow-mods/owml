using System;

namespace OWML.Common
{
    public interface IHarmonyHelper
    {
        void AddPrefix<T>(string methodName, Type patchType, string patchMethodName);
        void AddPostfix<T>(string methodName, Type patchType, string patchMethodName);
        void EmptyMethod<T>(string methodName);
        void Transpile<T>(string methodName, Type patchType, string patchMethodName);
    }
}
