using OWML.Common;
using OWML.Utils;
using System.Collections.Generic;
using UnityEngine.InputSystem;
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
			string primaryKeyboardKeybind,
			string primaryGamepadKeybind)
		{
			var uniqueName = _manifest.UniqueName + name;

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);

			var primaryName = uniqueName + "Primary";
			var primaryAction = AddAction(_manifest, primaryName);
			AddBinding(_manifest, primaryAction, primaryKeyboardKeybind, InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, primaryAction, primaryGamepadKeybind, InputConsts.InputControlSchemes.GAMEPAD);

			inputCommandData.TrySetAsAxis(rebindableId, primaryName);

			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			Rebindables.Add((rebindableId, name, tooltip));

			return commandType;
		}

		public InputConsts.InputCommandType RegisterRebindable(
			string name,
			string tooltip,
			string primaryKeyboardKeybind,
			string primaryGamepadKeybind,
			string secondaryKeyboardKeybind,
			string secondaryGamepadKeybind)
		{
			var uniqueName = _manifest.UniqueName + name;

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);

			var primaryName = uniqueName + "Primary";
			var secondaryName = uniqueName + "Secondary";
			var primaryAction = AddAction(_manifest, primaryName);
			var secondaryAction = AddAction(_manifest, secondaryName);
			AddBinding(_manifest, primaryAction, primaryKeyboardKeybind, InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, primaryAction, primaryGamepadKeybind, InputConsts.InputControlSchemes.GAMEPAD);
			AddBinding(_manifest, secondaryAction, secondaryKeyboardKeybind, InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, secondaryAction, secondaryGamepadKeybind, InputConsts.InputControlSchemes.GAMEPAD);

			inputCommandData.TrySetAsAxis(rebindableId, primaryName, secondaryName);

			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			Rebindables.Add((rebindableId, name, tooltip));

			return commandType;
		}

		private InputAction AddAction(IModManifest manifest, string name)
		{
			if (!OWMLRebinding.CustomActionMaps.ContainsKey(manifest.UniqueName))
			{
				OWMLRebinding.CustomActionMaps.Add(manifest.UniqueName, new InputActionMap(manifest.UniqueName));
			}

			return OWMLRebinding.CustomActionMaps[manifest.UniqueName].AddAction(
				name,
				InputActionType.Button,
				interactions: "OWInput",
				processors: "OWAxis",
				expectedControlLayout: "Button");
		}

		private void AddBinding(IModManifest manifest, InputAction action, string control, string inputControlScheme)
		{
			OWMLRebinding.CustomActionMaps[manifest.UniqueName].AddBinding(control, action, groups: inputControlScheme);
		}
	}
}