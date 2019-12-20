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
                    _harmonyHelper.AddPrefix<T>("Awake", typeof(Patches), nameof(Patches.BeforeAwake));
                    break;
                case Common.Events.AfterAwake:
                    _harmonyHelper.AddPostfix<T>("Awake", typeof(Patches), nameof(Patches.AfterAwake));
                    break;

                case Common.Events.BeforeStart:
                    _harmonyHelper.AddPrefix<T>("Start", typeof(Patches), nameof(Patches.BeforeStart));
                    break;
                case Common.Events.AfterStart:
                    _harmonyHelper.AddPostfix<T>("Start", typeof(Patches), nameof(Patches.AfterStart));
                    break;

                case Common.Events.BeforeEnable:
                    _harmonyHelper.AddPrefix<T>("OnEnable", typeof(Patches), nameof(Patches.BeforeEnable));
                    break;
                case Common.Events.AfterEnable:
                    _harmonyHelper.AddPostfix<T>("OnEnable", typeof(Patches), nameof(Patches.AfterEnable));
                    break;

                case Common.Events.BeforeDisable:
                    _harmonyHelper.AddPrefix<T>("OnDisable", typeof(Patches), nameof(Patches.BeforeDisable));
                    break;
                case Common.Events.AfterDisable:
                    _harmonyHelper.AddPostfix<T>("OnDisable", typeof(Patches), nameof(Patches.AfterDisable));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ev), ev, null);
            }
        }

    }
}
