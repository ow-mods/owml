using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OWML.Utils;

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
        private readonly HashSet<SingleAxisCommand> _toRemove = new HashSet<SingleAxisCommand>();
        private readonly Dictionary<SingleAxisCommand, bool> _wasPressed = new Dictionary<SingleAxisCommand, bool>();
        private readonly Dictionary<SingleAxisCommand, bool> _isPressed = new Dictionary<SingleAxisCommand, bool>();

        public void AddToListener(SingleAxisCommand command)
        {
            if (_commands.Contains(command))
            {
                return;
            }
            _commands.Add(command);
            _wasPressed.Add(command, false);
            _isPressed.Add(command, false);
        }

        public void RemoveFromListener(SingleAxisCommand command)
        {
            if (_commands.Contains(command) && !_toRemove.Contains(command))
            {
                _toRemove.Add(command);
            }
        }

        public void BlockNextRelease()
        {
            _commands.Where(x => x.IsPressed()).ToList().ForEach(x => x.BlockNextRelease());
        }

        public void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Update()
        {
            foreach (var command in _toRemove)
            {
                _commands.Remove(command);
                _isPressed.Remove(command);
                _wasPressed.Remove(command);
            }
            _toRemove.Clear();
            foreach (var command in _commands)
            {
                if (_toRemove.Contains(command))
                {
                    continue;
                }
                var blockFlag = command.GetValue<bool>("_blockNextRelease");
                if (command.IsNewlyPressed())
                {
                    OnNewlyPressed?.Invoke(command);
                }
                if (command.IsNewlyHeld(MinimalPressDuration))
                {
                    OnNewlyHeld?.Invoke(command);
                }
                if (command.IsPressed())
                {
                    OnPressed?.Invoke(command);
                    _isPressed[command] = true;
                }
                else
                {
                    _wasPressed[command] = _isPressed[command];
                    _isPressed[command] = false;
                }
                if (command.IsNewlyReleased())
                {
                    OnNewlyReleased?.Invoke(command);
                }
                if (command.IsHeld(MinimalPressDuration))
                {
                    OnHeld?.Invoke(command);
                }
                if (command.IsTapped(MaximalTapDuration))
                {
                    OnTapped?.Invoke(command);
                }
                if (blockFlag)//damn you, Mobius Digital
                {
                    var toReblock = _wasPressed[command] || _isPressed[command];
                    command.SetValue("_blockNextRelease", toReblock);
                }
            }
        }
    }
}
