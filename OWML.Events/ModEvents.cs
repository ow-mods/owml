using System;
using OWML.Common;
using UnityEngine;

namespace OWML.Events
{
    public class ModEvents : IModEvents
    {
        public Action<MonoBehaviour, Common.Events> OnEvent
        {
            get => Patches.OnEvent;
            set => Patches.OnEvent += value;
        }

        private readonly IHarmonyHelper _harmonyHelper;

        public ModEvents(IHarmonyHelper harmonyHelper)
        {
            _harmonyHelper = harmonyHelper;
        }

        public void AddEvent<T>(Common.Events ev) where T : MonoBehaviour
        {
            switch (ev)
            {
                case Common.Events.BeforeAwake:
                    {
                        _harmonyHelper.AddPrefix<T>("Awake", nameof(Patches.PreAwake));
                        break;
                    }
                case Common.Events.BeforeStart:
                    {
                        _harmonyHelper.AddPrefix<T>("Awake", nameof(Patches.PreStart));
                        break;
                    }
                case Common.Events.AfterAwake:
                    {
                        _harmonyHelper.AddPostfix<T>("Start", nameof(Patches.PostAwake));
                        break;
                    }
                case Common.Events.AfterStart:
                    {
                        _harmonyHelper.AddPostfix<T>("Start", nameof(Patches.PostStart));
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(ev), ev, null);
                    }
            }
        }

    }
}
