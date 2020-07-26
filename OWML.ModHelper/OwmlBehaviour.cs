using System;
using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
    public class OwmlBehaviour : MonoBehaviour
    {
        private static List<Action> _actions = new List<Action>();

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            ModConsole.Instance.WriteLine("", MessageType.Quit);
        }

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
