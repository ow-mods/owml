using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace OWML.Common
{
	public interface IRebindingHelper
	{
		public List<RebindableID> Rebindables { get; }

		public InputConsts.InputCommandType RegisterRebindableAxis(string name, string primary);

		public InputAction RegisterCustomAction(string name);

		public void AddBinding(InputAction action, string control);
	}
}
