using System;
using OWML.Common;
using UnityEngine;

namespace OWML.Events
{
    public class ModEvents : IModEvents
    {
        public Action<MonoBehaviour> OnAwake
        {
            get => Patches.OnAwake;
            set => Patches.OnAwake += value;
        }

        public Action<MonoBehaviour> OnStart
        {
            get => Patches.OnStart;
            set => Patches.OnStart += value;
        }

        private readonly IHarmonyHelper _harmonyHelper;

        public ModEvents(IHarmonyHelper harmonyHelper)
        {
            _harmonyHelper = harmonyHelper;
        }

        public void AddAwakeEvent<T>() where T : MonoBehaviour
        {
            _harmonyHelper.AddPostfix<T>("Awake", nameof(Patches.PostAwake));
        }

        public void AddStartEvent<T>() where T : MonoBehaviour
        {
            _harmonyHelper.AddPostfix<T>("Start", nameof(Patches.PostStart));
        }

    }
}
