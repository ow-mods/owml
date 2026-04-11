using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace OWML.Common
{
	public interface IRebindingHelper
	{
		public List<(RebindableID id, string name, string tooltip)> Rebindables { get; }

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string primaryKeybind,
			string secondaryKeybind = null,
			string tooltip = null);
	}
}