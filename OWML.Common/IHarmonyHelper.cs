using System;
using UnityEngine;

namespace OWML.Common
{
    public interface IHarmonyHelper
    {
        void AddPostfix<T>(string methodName, string patchName) where T : MonoBehaviour;
        void EmptyMethod<T>(string methodName) where T : MonoBehaviour;
        void Transpile<T>(string methodName, Type patchType, string patchMethodName) where T : MonoBehaviour;
    }
}
