using System;
using UnityEngine;

namespace OWML.Common
{
    public interface IModEvents
    {
        event Action<MonoBehaviour, Events> OnEvent;

        void Subscribe<T>(Events ev) where T : MonoBehaviour;
    }
}
