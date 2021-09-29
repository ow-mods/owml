using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModCommandListener : MonoBehaviour
    {
        //private readonly HashSet<SingleAxisCommand> _commands = new();

        //public event Action<SingleAxisCommand> OnNewlyPressed;
        //public event Action<SingleAxisCommand> OnNewlyReleased;

        //public void AddToListener(SingleAxisCommand command)
        //{
        //	if (_commands.Contains(command))
        //	{
        //		return;
        //	}
        //	_commands.Add(command);
        //}

        //private void Update()
        //{
        //foreach (var command in _commands)
        //{
        //	if (OWInput.IsNewlyPressed(command))
        //	{
        //		OnNewlyPressed?.Invoke(command);
        //	}

        //	if (OWInput.IsNewlyReleased(command))
        //	{
        //		OnNewlyReleased?.Invoke(command);
        //	}
        //}
        //}
    }
}
