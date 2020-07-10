using System;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModCommandListener : MonoBehaviour
    {
        public event Action<SingleAxisCommand> OnNewlyPressed;
        public event Action<SingleAxisCommand> OnNewlyReleased;
        public event Action<SingleAxisCommand> OnNewlyHeld;
        public event Action<SingleAxisCommand> OnPressed;
        public event Action<SingleAxisCommand> OnTapped;
        public event Action<SingleAxisCommand> OnHeld;

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
                OnNewlyPressed?.Invoke(_command);
            }
            if (_command.IsNewlyHeld(_minPressDuration))
            {
                OnNewlyHeld?.Invoke(_command);
            }
            if (_command.IsNewlyReleased())
            {
                OnNewlyReleased?.Invoke(_command);
            }
            if (_command.IsPressed())
            {
                OnPressed?.Invoke(_command);
            }
            if (_command.IsHeld(_minPressDuration))
            {
                OnHeld?.Invoke(_command);
            }
            if (_command.IsTapped(_maxTapDuration))
            {
                OnTapped?.Invoke(_command);
            }
        }
    }
}