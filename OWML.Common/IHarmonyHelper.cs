using System;
using UnityEngine;

namespace OWML.Common
{
    public interface IHarmonyHelper
    {
        void AddPrefix<T>(string methodName, Type patchType, string patchMethodName) where T : MonoBehaviour;
        void AddPostfix<T>(string methodName, Type patchType, string patchMethodName) where T : MonoBehaviour;
        void EmptyMethod<T>(string methodName) where T : MonoBehaviour;
        void Transpile<T>(string methodName, Type patchType, string patchMethodName) where T : MonoBehaviour;
    }
}
