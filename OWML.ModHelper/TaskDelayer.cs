using System;
using System.Collections.Generic;
using UnityEngine;

namespace OWML.ModHelper
{
    public class TaskDelayer : MonoBehaviour
    {
        private static List<Action> _actions = new List<Action>();

        public static void FireOnNextUpdate(Action action)
        {
            _actions.Add(action);
        }

        private void Update()
        {
            _actions.ForEach(action => action.Invoke());
            _actions = new List<Action>();
        }
    }
}
