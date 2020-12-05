using System;
using OWML.Common.Interfaces;
using UnityEngine;

namespace OWML.ModHelper.Events
{
    public class ModPlayerEvents : IModPlayerEvents
    {
        public event Action<PlayerBody> OnPlayerAwake;

        public void Init(IModEvents events)
        {
            events.Subscribe<PlayerBody>(Common.Enums.Events.AfterAwake);
            events.Event += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Enums.Events ev)
        {
            if (behaviour is PlayerBody playerBody && ev == Common.Enums.Events.AfterAwake)
            {
                OnPlayerAwake?.Invoke(playerBody);
            }
        }
    }
}
