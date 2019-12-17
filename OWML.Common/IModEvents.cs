using System;
using UnityEngine;

namespace OWML.Common
{
    public interface IModEvents
    {
        Action<MonoBehaviour> OnAwake { get; set; }
        Action<MonoBehaviour> OnStart { get; set; }

        void AddAwakeEvent<T>() where T : MonoBehaviour;
        void AddStartEvent<T>() where T : MonoBehaviour;
    }
}
