using System;
using System.Collections.Generic;
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

        public float MinimalPressDuration { get; set; } = 0.1f;
        public float MaximalTapDuration { get; set; } = 0.1f;

        private readonly HashSet<SingleAxisCommand> _commands = new HashSet<SingleAxisCommand>();

        public void AddToListener(SingleAxisCommand command)
        {
            if (!_commands.Contains(command))
            {
                _commands.Add(command);
            }
        }

        public void RemoveFromListener(SingleAxisCommand command)
        {
            if (_commands.Contains(command))
            {
                _commands.Remove(command);
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
                if (command.IsNewlyHeld(MinimalPressDuration))
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
                if (command.IsHeld(MinimalPressDuration))
                {
                    OnHeld?.Invoke(command);
                }
                if (command.IsTapped(MaximalTapDuration))
                {
                    OnTapped?.Invoke(command);
                }
            }
        }
    }
}
