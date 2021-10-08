using System;
using System.Collections.Generic;
using UnityEngine;

namespace OWML.ModHelper.Input
{
	public class ModCommandListener : MonoBehaviour
	{
		private readonly HashSet<IInputCommands> _commands = new();

		public event Action<IInputCommands> OnNewlyPressed;
		public event Action<IInputCommands> OnNewlyReleased;

		public void AddToListener(IInputCommands command)
		{
			if (_commands.Contains(command))
			{
				return;
			}
			_commands.Add(command);
		}

		private void Update()
		{
			foreach (var command in _commands)
			{
				if (OWInput.IsNewlyPressed(command))
				{
					OnNewlyPressed?.Invoke(command);
				}

				if (OWInput.IsNewlyReleased(command))
				{
					OnNewlyReleased?.Invoke(command);
				}
			}
		}
	}
}
