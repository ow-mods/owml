using System;
using OWML.Common;
using OWML.Utils;
using System.Collections.Generic;
using System.Data;
using OWML.Common.Enums;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using static InputCommandDefinitions;

namespace OWML.ModHelper.Input
{
	public class RebindingHelper : IRebindingHelper
	{
		public List<(RebindableID id, string name, string tooltip)> Rebindables { get; } = new();

		private readonly IModConsole _console;
		private readonly IModManifest _manifest;

		public RebindingHelper(IModConsole console, IModManifest manifest)
		{
			_console = console;
			_manifest = manifest;
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

			var primaryName = uniqueName + "Primary";
			var primaryAction = AddAction(_manifest, primaryName, axis);
			AddBinding(_manifest, primaryAction, Keyboard.current[keyboardKeybind].path, InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, primaryAction, GetGamepadPath(gamepadKeybind), InputConsts.InputControlSchemes.GAMEPAD);

			inputCommandData.TrySetAsAxis(rebindableId, primaryName);

			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			Rebindables.Add((rebindableId, name, tooltip));

			((InputManager)OWInput.SharedInputManager).commandManager.OnInputCommandsInitialized += () =>
			{
				_console.WriteLine($"OnInputCommandsInitialized -- setting {commandType} to {pressedThreshold}");
				var command = InputLibrary.GetInputCommand(commandType);
				command.PressedThreshold = pressedThreshold;
			};

			return commandType;
		}

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			Key primaryKeyboardKeybind,
			GamepadBinding primaryGamepadKeybind,
			Key secondaryKeyboardKeybind,
			GamepadBinding secondaryGamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f)
		{
			var uniqueName = _manifest.UniqueName + name;

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);

			var primaryName = uniqueName + "Primary";
			var secondaryName = uniqueName + "Secondary";
			var primaryAction = AddAction(_manifest, primaryName, axis);
			var secondaryAction = AddAction(_manifest, secondaryName, axis);
			AddBinding(_manifest, primaryAction, Keyboard.current[primaryKeyboardKeybind].path, InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, primaryAction, GetGamepadPath(primaryGamepadKeybind), InputConsts.InputControlSchemes.GAMEPAD);
			AddBinding(_manifest, secondaryAction, Keyboard.current[secondaryKeyboardKeybind].path, InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, secondaryAction, GetGamepadPath(secondaryGamepadKeybind), InputConsts.InputControlSchemes.GAMEPAD);

			inputCommandData.TrySetAsAxis(rebindableId, primaryName, secondaryName);

			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			Rebindables.Add((rebindableId, name, tooltip));

			((InputManager)OWInput.SharedInputManager).commandManager.OnInputCommandsInitialized += () =>
			{
				_console.WriteLine($"OnInputCommandsInitialized -- setting {commandType} to {pressedThreshold}");

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

			var primaryName = EnumUtils.GetName(typeof(InputConsts.InputCommandType), yAxis);
			var secondaryName = EnumUtils.GetName(typeof(InputConsts.InputCommandType), xAxis);

			_console.WriteLine($"RegisterComposite primary:{primaryName} secondary:{secondaryName}");

			var primaryId = (RebindableID)EnumUtils.Parse(typeof(RebindableID), primaryName);
			var secondaryId = (RebindableID)EnumUtils.Parse(typeof(RebindableID), secondaryName);

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Composite, commandType);
			inputCommandData.TrySetAsComposite(primaryId, primaryName + "Primary", primaryName + "Secondary", secondaryId, secondaryName + "Primary", secondaryName + "Secondary");
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

			_console.WriteLine($"AddAction {name}");

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