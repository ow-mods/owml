using System;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Events
{
    public class ModPlayerEvents : IModPlayerEvents
    {
        public event Action<PlayerBody> OnPlayerAwake;

        public ModPlayerEvents(IModEvents events)
        {
            events.Subscribe<PlayerBody>(Common.Events.AfterAwake);
            events.OnEvent2 += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
        {
            if (behaviour is PlayerBody playerBody && ev == Common.Events.AfterAwake)
            {
                OnPlayerAwake?.Invoke(playerBody);
            }
        }
    }
}
