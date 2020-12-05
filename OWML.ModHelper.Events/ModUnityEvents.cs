using System;
using System.Collections;
using OWML.Common.Interfaces;
using UnityEngine;

namespace OWML.ModHelper.Events
{
    public class ModUnityEvents : MonoBehaviour, IModUnityEvents
    {
        public event Action OnUpdate;

        public event Action OnFixedUpdate;

        public event Action OnLateUpdate;

        public void FireOnNextUpdate(Action action)
        {
            FireInNUpdates(action, 1);
        }

        public void FireInNUpdates(Action action, int n)
        {
            StartCoroutine(WaitForFrames(action, n));
        }

        public void RunWhen(Func<bool> predicate, Action action)
        {
            StartCoroutine(WaitUntil(predicate, action));
        }

        private IEnumerator WaitForFrames(Action action, int n)
        {
            for (var i = 0; i < n; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            action.Invoke();
        }

        private IEnumerator WaitUntil(Func<bool> predicate, Action action)
        {
            yield return new WaitUntil(predicate);
            action();
        }

        private void Start()
        {
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
