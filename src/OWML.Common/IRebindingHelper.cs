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
		/// Register a key rebind.
		/// </summary>
		/// <param name="name">The name of the input. Showed in the mod config menu. Must be unique in your mod.</param>
		/// <param name="tooltip">The tooltip showed in the mod config menu.</param>
		/// <param name="keyboardKeybind">The default keyboard binding.</param>
		/// <param name="gamepadKeybind">The default gamepad binding.</param>
		/// <param name="axis">Should be true for inputs that expect an analog input - eg triggers. For simple buttons, this should be false. See the docs for more info.</param>
		/// <param name="pressedThreshold">Used to control when <see cref="OWInput.IsPressed"/> returns true - see docs for more info.</param>
		/// <returns>An <see cref="InputConsts.InputCommandType"/> used in <see cref="InputLibrary.GetInputCommand"/> or <see cref="RegisterComposite"/>.</returns>
		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			Key keyboardKeybind,
			GamepadBinding gamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f);

		/// <summary>
		/// Register a key rebind.
		/// </summary>
		/// <param name="name">The name of the input. Showed in the mod config menu. Must be unique in your mod.</param>
		/// <param name="tooltip">The tooltip showed in the mod config menu.</param>
		/// <param name="mouseKeybind">The default mouse binding.</param>
		/// <param name="gamepadKeybind">The default gamepad binding.</param>
		/// <param name="axis">Should be true for inputs that expect an analog input - eg triggers. For simple buttons, this should be false. See the docs for more info.</param>
		/// <param name="pressedThreshold">Used to control when <see cref="OWInput.IsPressed"/> returns true - see docs for more info.</param>
		/// <returns>An <see cref="InputConsts.InputCommandType"/> used in <see cref="InputLibrary.GetInputCommand"/> or <see cref="RegisterComposite"/>.</returns>
		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			MouseBinding mouseKeybind,
			GamepadBinding gamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f);

		/// <summary>
		/// Register a key rebind with a positive and negative action.
		/// </summary>
		/// <param name="name">The name of the input. Showed in the mod config menu. Must be unique in your mod.</param>
		/// <param name="tooltip">The tooltip showed in the mod config menu.</param>
		/// <param name="positiveKeyboardKeybind">The default keyboard binding for the positive (ie. right, up) action.</param>
		/// <param name="positiveGamepadKeybind">The default gamepad binding for the positive (ie. right, up) action.</param>
		/// <param name="negativeKeyboardKeybind">The default keyboard binding for the negative (ie. left, down) action.</param>
		/// <param name="negativeGamepadKeybind">The default gamepad binding for the negative (ie. left, down) action.</param>
		/// <param name="axis">Should be true for inputs that expect an analog input - eg triggers. For simple buttons, this should be false. See the docs for more info.</param>
		/// <param name="pressedThreshold">Used to control when <see cref="OWInput.IsPressed"/> returns true - see docs for more info.</param>
		/// <returns>An <see cref="InputConsts.InputCommandType"/> used in <see cref="InputLibrary.GetInputCommand"/> or <see cref="RegisterComposite"/>.</returns>
		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			Key positiveKeyboardKeybind,
			GamepadBinding positiveGamepadKeybind,
			Key negativeKeyboardKeybind,
			GamepadBinding negativeGamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f);

		/// <summary>
		/// Register a key rebind with a positive and negative action.
		/// </summary>
		/// <param name="name">The name of the input. Showed in the mod config menu. Must be unique in your mod.</param>
		/// <param name="tooltip">The tooltip showed in the mod config menu.</param>
		/// <param name="positiveMouseKeybind">The default mouse binding for the positive (ie. right, forward) action.</param>
		/// <param name="positiveGamepadKeybind">The default gamepad binding for the positive (ie. right, up) action.</param>
		/// <param name="negativeMouseKeybind">The default mouse binding for the negative (ie. left, back) action.</param>
		/// <param name="negativeGamepadKeybind">The default gamepad binding for the negative (ie. left, down) action.</param>
		/// <param name="axis">Should be true for inputs that expect an analog input - eg triggers. For simple buttons, this should be false. See the docs for more info.</param>
		/// <param name="pressedThreshold">Used to control when <see cref="OWInput.IsPressed"/> returns true - see docs for more info.</param>
		/// <returns>An <see cref="InputConsts.InputCommandType"/> used in <see cref="InputLibrary.GetInputCommand"/> or <see cref="RegisterComposite"/>.</returns>
		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			MouseBinding positiveMouseKeybind,
			GamepadBinding positiveGamepadKeybind,
			MouseBinding negativeMouseKeybind,
			GamepadBinding negativeGamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f);

		/// <summary>
		/// Register a 2D input from two 1D inputs.
		/// </summary>
		/// <param name="name">The name of the input. Showed in the mod config menu. Must be unique in your mod.</param>
		/// <param name="xAxis">The X axis.</param>
		/// <param name="yAxis">The Y axis.</param>
		/// <returns>An <see cref="InputConsts.InputCommandType"/> used in <see cref="InputLibrary.GetInputCommand"/>.</returns>
		public InputConsts.InputCommandType RegisterComposite(
			string name,
			InputConsts.InputCommandType xAxis,
			InputConsts.InputCommandType yAxis);
	}
}