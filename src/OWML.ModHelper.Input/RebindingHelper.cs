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
			KeyOrMouse kbmKeybind,
			GamepadBinding gamepadKeybind)
		{
			var positiveName = name + (positive ? "Positive" : "Negative");

			return CreateActionAndBinding(_manifest, positiveName, axis, kbmKeybind, gamepadKeybind);
		}

		private string CreateActionAndBinding(
			IModManifest manifest,
			string name,
			bool axis,
			KeyOrMouse kbmKeybind,
			GamepadBinding gamepadKeybind)
		{
			var action = AddAction(_manifest, name, axis);

			AddBinding(_manifest, action, kbmKeybind.GetInputCommandPath(), InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, action, gamepadKeybind.GetInputCommandPath(), InputConsts.InputControlSchemes.GAMEPAD);

			return name;
		}

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			Key keyboardKeybind,
			GamepadBinding gamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f)
		{
			return RegisterRebindable(
				name,
				tooltip,
				new KeyOrMouse(keyboardKeybind),
				gamepadKeybind,
				axis,
				pressedThreshold
			);
		}
		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			MouseBinding mouseKeybind,
			GamepadBinding gamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f)
		{
			return RegisterRebindable(
				name,
				tooltip,
				new KeyOrMouse(mouseKeybind),
				gamepadKeybind,
				axis,
				pressedThreshold
			);
		}

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			KeyOrMouse kbmKeybind,
			GamepadBinding gamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f)
		{
			var uniqueName = _manifest.UniqueName + name;
			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);

			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);

			var action = CreateActionAndBinding(_manifest, uniqueName, axis, kbmKeybind, gamepadKeybind);

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
			return RegisterRebindable(
				name,
				tooltip,
				new KeyOrMouse(positiveKeyboardKeybind),
				positiveGamepadKeybind,
				new KeyOrMouse(negativeKeyboardKeybind),
				negativeGamepadKeybind,
				axis,
				pressedThreshold
			);
		}

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			MouseBinding positiveMouseKeybind,
			GamepadBinding positiveGamepadKeybind,
			MouseBinding negativeMouseKeybind,
			GamepadBinding negativeGamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f)
		{
			return RegisterRebindable(
				name,
				tooltip,
				new KeyOrMouse(positiveMouseKeybind),
				positiveGamepadKeybind,
				new KeyOrMouse(negativeMouseKeybind),
				negativeGamepadKeybind,
				axis,
				pressedThreshold
			);
		}

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			KeyOrMouse positiveKbmKeybind,
			GamepadBinding positiveGamepadKeybind,
			KeyOrMouse negativeKbmKeybind,
			GamepadBinding negativeGamepadKeybind,
			bool axis,
			float pressedThreshold = 0.4f)
		{
			var uniqueName = _manifest.UniqueName + name;
			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);

			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);

			var positiveAction = CreateActionAndBinding(_manifest, uniqueName, true, axis, positiveKbmKeybind, positiveGamepadKeybind);
			var negativeAction = CreateActionAndBinding(_manifest, uniqueName, false, axis, negativeKbmKeybind, negativeGamepadKeybind);

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
			InputConsts.InputCommandType xAxis,
			InputConsts.InputCommandType yAxis)
		{
			var uniqueName = _manifest.UniqueName + name;

			var yName = EnumUtils.GetName<InputConsts.InputCommandType>(yAxis);
			var xName = EnumUtils.GetName<InputConsts.InputCommandType>(xAxis);

			var yId = EnumUtils.Parse<RebindableID>(yName);
			var xId = EnumUtils.Parse<RebindableID>(xName);

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Composite, commandType);
			inputCommandData.TrySetAsComposite(
				yId, yName + "Positive", yName + "Negative",
				xId, xName + "Positive", xName + "Negative");
			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			return commandType;
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

		public void MarkAsXAxis(RebindableID id)
		{
			OWMLRebinding.XAxisRebindables.Add(id);
		}

		private static RebindableID InputCommandToRebindable(InputConsts.InputCommandType inputCommandType)
		{
			var name = EnumUtils.GetName<InputConsts.InputCommandType>(inputCommandType);
			return EnumUtils.Parse<RebindableID>(name);
		}

		public void MarkAsXAxis(InputConsts.InputCommandType xAxis)
			=> MarkAsXAxis(InputCommandToRebindable(xAxis));

		public readonly struct KeyOrMouse
		{
			public readonly Key? key;
			public readonly MouseBinding? mouse;

			public KeyOrMouse(Key key) => (this.key, this.mouse) = (key, null);
			public KeyOrMouse(MouseBinding mouse) => (this.key, this.mouse) = (null, mouse);

			public string GetInputCommandPath()
			{
				if (key != null)
					return key.Value.GetInputCommandPath();

				if (mouse != null)
					return mouse.Value.GetInputCommandPath();

				throw new InvalidOperationException();
			}
		}
	}
}