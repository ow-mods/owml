using System;
using OWML.Common.Interfaces;
using UnityEngine;

namespace OWML.UnityAbstractions
{
    public class GameObjectHelper : IGameObjectHelper
    {
        public T CreateAndAdd<T>(string name = null) where T : MonoBehaviour
        {
            var go = new GameObject(name);
            return go.AddComponent<T>();
        }

        public object CreateAndAdd(Type type, string name = null)
        {
            var go = new GameObject(name);
            return go.AddComponent(type);
        }
    }
}
