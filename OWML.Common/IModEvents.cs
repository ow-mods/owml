using System;
using UnityEngine;

namespace OWML.Common
{
    public interface IModEvents
    {
        Action<MonoBehaviour, Events> OnEvent { get; set; }

        void AddEvent<T>(Events ev) where T : MonoBehaviour;
    }
}
