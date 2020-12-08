using System;
using UnityEngine;

namespace OWML.Common.Interfaces
{
    public interface IGameObjectHelper
    {
        TInterface CreateAndAdd<TInterface, TBehaviour>(string name = null) where TBehaviour : MonoBehaviour;

        TBehaviour CreateAndAdd<TBehaviour>(string name = null) where TBehaviour : MonoBehaviour;

        TBehaviour CreateAndAdd<TBehaviour>(Type type, string name = null);
    }
}