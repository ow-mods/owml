using System;
using System.Collections;
using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Events
{
    public class ModUnityEvents : MonoBehaviour, IModUnityEvents
    {
        private bool _isStarted;
        private readonly List<Action> _earlyActions = new List<Action>();

        public event Action OnUpdate;
        public event Action OnFixedUpdate;
        public event Action OnLateUpdate;

        public void FireOnNextUpdate(Action action)
        {
            if (_isStarted)
            {
                StartCoroutine(WaitOneFrame(action));
            }
            else
            {
                _earlyActions.Add(action);
            }
        }

        private IEnumerator WaitOneFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }

        private void Start()
        {
            _isStarted = true;
            _earlyActions.ForEach(action => StartCoroutine(WaitOneFrame(action)));
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

    }
}
