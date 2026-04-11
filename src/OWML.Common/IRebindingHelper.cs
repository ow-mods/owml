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
			string primaryGamepadKeybind);

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			string primaryKeyboardKeybind,
			string primaryGamepadKeybind,
			string secondaryKeyboardKeybind,
			string secondaryGamepadKeybind);
	}
}