using System;
using OWML.Common.Interfaces;
using UnityEngine;

namespace OWML.Abstractions
{
    public class GameObjectHelper : IGameObjectHelper
    {
        public TInterface CreateAndAdd<TInterface, TBehaviour>(string name = null) where TBehaviour : MonoBehaviour => 
            (TInterface)(object)CreateAndAdd<TBehaviour>(name);

        public TBehaviour CreateAndAdd<TBehaviour>(string name = null) where TBehaviour : MonoBehaviour => 
            new GameObject(name).AddComponent<TBehaviour>();

        public TBehaviour CreateAndAdd<TBehaviour>(Type type, string name = null) => 
            (TBehaviour)(object)new GameObject(name).AddComponent(type);
    }
}
