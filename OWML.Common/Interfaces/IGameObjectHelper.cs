using System;
using UnityEngine;

namespace OWML.Common.Interfaces
{
    public interface IGameObjectHelper
    {
        T CreateAndAdd<T>(string name = null) where T : MonoBehaviour;

        object CreateAndAdd(Type type, string name = null);
    }
}