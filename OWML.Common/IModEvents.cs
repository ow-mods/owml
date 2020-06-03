using System;
using UnityEngine;

namespace OWML.Common
{
    /// <summary>
    /// Handler for subscribing to game events.
    /// </summary>
    public interface IModEvents
    {
        Action<MonoBehaviour, Events> OnEvent { get; set; }

        /// <summary>Subscribe to the given event.</summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="ev">The event to subscribe to.</param>
        void Subscribe<T>(Events ev) where T : MonoBehaviour;

        [Obsolete("Use Subscribe instead")]
        void AddEvent<T>(Events ev) where T : MonoBehaviour;
    }
}
