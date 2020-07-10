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

        private float _minPressDuration = 0.1f, _maxTapDuration = 0.1f;
        private HashSet<SingleAxisCommand> _commands = new List<SingleAxisCommand>();

        public float MinimalPressDuration 
        {
            get
            {
                return _minPressDuration;
            }
            set
            {
                _minPressDuration = value;
            }
        }
        
        public float MaximalTapDuration 
        {
            get
            {
                return _maxTapDuration;
            }
            set
            {
                _maxTapDuration = value;
            }
        }

        public void AddToListener(SingleAxisCommand command)
        {
            if (!_commands.Contains(command))
            {
                _commands.Add(command);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            foreach (var command in _commands)
            {
                if (command.IsNewlyPressed())
                {
                    OnNewlyPressed?.Invoke(command);
                }
                if (command.IsNewlyHeld(_minPressDuration))
                {
                    OnNewlyHeld?.Invoke(command);
                }
                if (command.IsNewlyReleased())
                {
                    OnNewlyReleased?.Invoke(command);
                }
                if (command.IsPressed())
                {
                    OnPressed?.Invoke(command);
                }
                if (command.IsHeld(_minPressDuration))
                {
                    OnHeld?.Invoke(command);
                }
                if (command.IsTapped(_maxTapDuration))
                {
                    OnTapped?.Invoke(command);
                }
            }
        }
    }
}
