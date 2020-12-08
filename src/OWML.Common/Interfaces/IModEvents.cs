using System;
using OWML.Common.Enums;
using UnityEngine;

namespace OWML.Common.Interfaces
{
    public interface IModEvents
    {
        IModPlayerEvents Player { get; }
        IModSceneEvents Scenes { get; }
        IModUnityEvents Unity { get; }

        event Action<MonoBehaviour, Events> Event;

        [Obsolete("Use Event instead.")]
        Action<MonoBehaviour, Events> OnEvent { get; set; }

        void Subscribe<T>(Events ev) where T : MonoBehaviour;
    }
}
