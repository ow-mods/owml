using System;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModCommandListener : MonoBehaviour
    {
        public event Action OnNewlyPressed;
        public event Action OnNewlyReleased;
        public event Action OnNewlyHeld;
        public event Action OnPressed;
        public event Action OnTapped;
        public event Action OnHeld;

        private float _minPressDuration, _maxTapDuration;
        private SingleAxisCommand _command;

        public void Initialize(SingleAxisCommand command, float minPressDuration = 0.1f, float maxTapDuration = 0.1f)
        {
            _command = command;
            _minPressDuration = minPressDuration;
            _maxTapDuration = maxTapDuration;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (_command == null)
            {
                return;
            }
            if (_command.IsNewlyPressed())
            {
                OnNewlyPressed?.Invoke();
            }
            if (_command.IsNewlyHeld(_minPressDuration))
            {
                OnNewlyHeld?.Invoke();
            }
            if (_command.IsNewlyReleased())
            {
                OnNewlyReleased?.Invoke();
            }
            if (_command.IsPressed())
            {
                OnPressed?.Invoke();
            }
            if (_command.IsHeld(_minPressDuration))
            {
                OnHeld?.Invoke();
            }
            if (_command.IsTapped(_maxTapDuration))
            {
                OnTapped?.Invoke();
            }
        }
    }
}