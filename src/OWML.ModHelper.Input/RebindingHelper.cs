using System;
using OWML.Common;
using OWML.Utils;
using System.Collections.Generic;
using OWML.Common.Enums;
using UnityEngine.InputSystem;

namespace OWML.ModHelper.Input
{
	public class RebindingHelper : IRebindingHelper
	{
		public List<(RebindableID id, string name, string tooltip)> Rebindables { get; } = new();

		private readonly IModManifest _manifest;

		public RebindingHelper(IModManifest manifest)
		{
			_manifest = manifest;
		}

		private string CreateActionAndBinding(
			IModManifest manifest,
			string name,
			bool positive,
			bool axis,
			Key keyboardKeybind,
			GamepadBinding gamepadKeybind)
		{
			var positiveName = name + (positive ? "Positive" : "Negative");
			var positiveAction = AddAction(_manifest, positiveName, axis);
			AddBinding(_manifest, positiveAction, Keyboard.current[keyboardKeybind].path, InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, positiveAction, GetGamepadPath(gamepadKeybind), InputConsts.InputControlSchemes.GAMEPAD);

			return positiveName;
		}

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			Key keyboardKeybind,
			GamepadBinding gamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f)
		{
			var uniqueName = _manifest.UniqueName + name;
			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);

			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);
			var action = CreateActionAndBinding(_manifest, uniqueName, true, axis, keyboardKeybind, gamepadKeybind);
			inputCommandData.TrySetAsAxis(rebindableId, action);
			InputCommandDefinitions.AddInputCommandData(inputCommandData);
			Rebindables.Add((rebindableId, name, tooltip));

			((InputManager)OWInput.SharedInputManager).commandManager.OnInputCommandsInitialized += () =>
			{
				var command = InputLibrary.GetInputCommand(commandType);
				command.PressedThreshold = pressedThreshold;
			};

			return commandType;
		}

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			Key positiveKeyboardKeybind,
			GamepadBinding positiveGamepadKeybind,
			Key negativeKeyboardKeybind,
			GamepadBinding negativeGamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f)
		{
			var uniqueName = _manifest.UniqueName + name;
			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);

			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);
			var positiveAction = CreateActionAndBinding(_manifest, uniqueName, true, axis, positiveKeyboardKeybind, positiveGamepadKeybind);
			var negativeAction = CreateActionAndBinding(_manifest, uniqueName, false, axis, negativeKeyboardKeybind, negativeGamepadKeybind);
			inputCommandData.TrySetAsAxis(rebindableId, positiveAction, negativeAction);
			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			Rebindables.Add((rebindableId, name, tooltip));

			((InputManager)OWInput.SharedInputManager).commandManager.OnInputCommandsInitialized += () =>
			{
				var command = InputLibrary.GetInputCommand(commandType);
				command.PressedThreshold = pressedThreshold;
			};

			return commandType;
		}

		public InputConsts.InputCommandType RegisterComposite(
			string name, 
			InputConsts.InputCommandType yAxis,
			InputConsts.InputCommandType xAxis)
		{
			var uniqueName = _manifest.UniqueName + name;

			var yName = EnumUtils.GetName(typeof(InputConsts.InputCommandType), yAxis);
			var xName = EnumUtils.GetName(typeof(InputConsts.InputCommandType), xAxis);

			var yId = (RebindableID)EnumUtils.Parse(typeof(RebindableID), yName);
			var xId = (RebindableID)EnumUtils.Parse(typeof(RebindableID), xName);

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Composite, commandType);
			inputCommandData.TrySetAsComposite(
				yId, yName + "Positive", yName + "Negative",
				xId, xName + "Positive", xName + "Negative");
			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			return commandType;
		}

		private string GetGamepadPath(GamepadBinding binding)
		{
			switch (binding)
			{
				case GamepadBinding.LeftStickUp:
					return "<Gamepad>/leftStick/up";
				case GamepadBinding.LeftStickDown:
					return "<Gamepad>/leftStick/down";
				case GamepadBinding.LeftStickLeft:
					return "<Gamepad>/leftStick/left";
				case GamepadBinding.LeftStickRight:
					return "<Gamepad>/leftStick/right";

				case GamepadBinding.RightStickUp:
					return "<Gamepad>/rightStick/up";
				case GamepadBinding.RightStickDown:
					return "<Gamepad>/rightStick/down";
				case GamepadBinding.RightStickLeft:
					return "<Gamepad>/rightStick/left";
				case GamepadBinding.RightStickRight:
					return "<Gamepad>/rightStick/right";

				case GamepadBinding.Share:
					return "<Gamepad>/share";
				case GamepadBinding.SystemButton:
					return "<Gamepad>/systemButton";

				case GamepadBinding.DPadUp:
					return "<Gamepad>/dpad/up";
				case GamepadBinding.DPadDown:
					return "<Gamepad>/dpad/down";
				case GamepadBinding.DPadLeft:
					return "<Gamepad>/dpad/left";
				case GamepadBinding.DPadRight:
					return "<Gamepad>/dpad/right";

				case GamepadBinding.UpButton:
					return "<Gamepad>/buttonNorth";
				case GamepadBinding.LeftButton:
					return "<Gamepad>/buttonEast";
				case GamepadBinding.DownButton:
					return "<Gamepad>/buttonSouth";
				case GamepadBinding.RightButton:
					return "<Gamepad>/buttonWest";

				case GamepadBinding.LeftStickClick:
					return "<Gamepad>/leftStickPress";
				case GamepadBinding.RightStickClick:
					return "<Gamepad>/rightStickPress";

				case GamepadBinding.LeftShoulder:
					return "<Gamepad>/leftShoulder";
				case GamepadBinding.RightShoulder:
					return "<Gamepad>/rightShoulder";

				case GamepadBinding.Start:
					return "<Gamepad>/start";
				case GamepadBinding.Select:
					return "<Gamepad>/select";

				case GamepadBinding.LeftTrigger:
					return "<Gamepad>/leftTrigger";
				case GamepadBinding.RightTrigger:
					return "<Gamepad>/rightTrigger";

				default:
					throw new NotImplementedException();
			}
		}

		private InputAction AddAction(IModManifest manifest, string name, bool axis)
		{
			if (!OWMLRebinding.CustomActionMaps.ContainsKey(manifest.UniqueName))
			{
				OWMLRebinding.CustomActionMaps.Add(manifest.UniqueName, new InputActionMap(manifest.UniqueName));
			}

			return OWMLRebinding.CustomActionMaps[manifest.UniqueName].AddAction(
				name,
				axis ? InputActionType.Value : InputActionType.Button,
				interactions: "OWInput",
				processors: "OWAxis",
				expectedControlLayout: axis ? "Axis" : "Button");
		}

		private void AddBinding(IModManifest manifest, InputAction action, string control, string inputControlScheme)
		{
			OWMLRebinding.CustomActionMaps[manifest.UniqueName].AddBinding(control, action, groups: inputControlScheme);
		}
	}
}