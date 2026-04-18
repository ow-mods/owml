using OWML.Common.Enums;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace OWML.Common
{
	public interface IRebindingHelper
	{
		public List<(RebindableID id, string name, string tooltip)> Rebindables { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">The name of the input. Showed in the mod config menu. Must be unique in your mod.</param>
		/// <param name="tooltip">The tooltip showed in the mod config menu.</param>
		/// <param name="keyboardKeybind">The default keyboard binding.</param>
		/// <param name="gamepadKeybind">The default gamepad binding.</param>
		/// <param name="axis"></param>
		/// <param name="pressedThreshold"></param>
		/// <returns>An <see cref="InputConsts.InputCommandType"/> used in <see cref="InputLibrary.GetInputCommand"/>.</returns>
		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			Key keyboardKeybind,
			GamepadBinding gamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f);

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			Key primaryKeyboardKeybind,
			GamepadBinding primaryGamepadKeybind,
			Key secondaryKeyboardKeybind,
			GamepadBinding secondaryGamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f);

		public InputConsts.InputCommandType RegisterComposite(
			string name,
			InputConsts.InputCommandType primary,
			InputConsts.InputCommandType secondary);
	}
}