using System;

namespace OWML.Common
{
    public interface IHarmonyHelper
    {
        void AddPrefix<T>(string methodName, Type patchType, string patchMethodName, Type[] parameters = null);
        void AddPostfix<T>(string methodName, Type patchType, string patchMethodName, Type[] parameters = null);
        void EmptyMethod<T>(string methodName, Type[] parameters = null);
        void Transpile<T>(string methodName, Type patchType, string patchMethodName, Type[] parameters = null);
    }
}
