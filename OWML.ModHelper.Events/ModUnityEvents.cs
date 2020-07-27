using System;
using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Events
{
    public class ModUnityEvents : MonoBehaviour, IModUnityEvents
    {
        private List<Action> _actions = new List<Action>();

        public void FireOnNextUpdate(Action action)
        {
            _actions.Add(action);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            _actions.ForEach(action => action.Invoke());
            _actions = new List<Action>();
        }

    }
}
