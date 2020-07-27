﻿using System;
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
        private int _frames;

        public event Action OnUpdate;
        public event Action OnFixedUpdate;
        public event Action OnLateUpdate;

        public void FireOnNextUpdate(Action action, int frames = 1)
        {
            if (_isStarted)
            {
                StartCoroutine(WaitForFrames(action, frames));
            }
            else
            {
                _frames = frames;
                _earlyActions.Add(action);
            }
        }

        private IEnumerator WaitForFrames(Action action, int frames)
        {
            for (var i = 0; i < frames; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            action();
        }

        private void Start()
        {
            _isStarted = true;
            _earlyActions.ForEach(action => StartCoroutine(WaitForFrames(action, _frames)));
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
