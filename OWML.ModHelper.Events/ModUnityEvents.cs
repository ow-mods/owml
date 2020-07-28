using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Events
{
    public class ModUnityEvents : MonoBehaviour, IModUnityEvents
    {
        public event Action OnUpdate;
        public event Action OnFixedUpdate;
        public event Action OnLateUpdate;

        private Dictionary<int, List<Action>> _delayedActions;

        public void FireAfterNFrames(int count, Action action)
        {
            var frame = Time.frameCount + count;
            if (!_delayedActions.ContainsKey(frame))
            {
                _delayedActions.Add(frame, new List<Action>());
            }
            _delayedActions[frame].Add(action);
        }

        public void FireOnNextUpdate(Action action)
        {
            var frame = Time.frameCount;
            if (!_delayedActions.ContainsKey(frame))
            {
                _delayedActions.Add(frame, new List<Action>());
            }
            _delayedActions[frame].Add(action);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            var keysToActivate = _delayedActions.Keys.Where(key => key <= Time.frameCount).ToList();
            keysToActivate.ForEach(key => _delayedActions[key].ForEach(action => action.Invoke()));
            keysToActivate.ForEach(key => _delayedActions.Remove(key));
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
