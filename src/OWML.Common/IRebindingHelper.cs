using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace OWML.Common
{
	public interface IRebindingHelper
	{
		public List<(RebindableID id, string name, string tooltip)> Rebindables { get; }

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			string primaryKeyboardKeybind,
			string primaryGamepadKeybind,
			bool axis);

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			string primaryKeyboardKeybind,
			string primaryGamepadKeybind,
			string secondaryKeyboardKeybind,
			string secondaryGamepadKeybind,
			bool axis);

		public InputConsts.InputCommandType RegisterComposite(string name, string primaryName, string secondaryName);
	}
}