using OWML.Common;
using OWML.Utils;
using System.Collections.Generic;
using System.Data;
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
			string primaryGamepadKeybind,
			bool axis)
		{
			var uniqueName = _manifest.UniqueName + name;

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);

			var primaryName = uniqueName + "Primary";
			var primaryAction = AddAction(_manifest, primaryName, axis);
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
			string secondaryGamepadKeybind,
			bool axis)
		{
			var uniqueName = _manifest.UniqueName + name;

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);

			var primaryName = uniqueName + "Primary";
			var secondaryName = uniqueName + "Secondary";
			var primaryAction = AddAction(_manifest, primaryName, axis);
			var secondaryAction = AddAction(_manifest, secondaryName, axis);
			AddBinding(_manifest, primaryAction, primaryKeyboardKeybind, InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, primaryAction, primaryGamepadKeybind, InputConsts.InputControlSchemes.GAMEPAD);
			AddBinding(_manifest, secondaryAction, secondaryKeyboardKeybind, InputConsts.InputControlSchemes.KEYBOARDMOUSE);
			AddBinding(_manifest, secondaryAction, secondaryGamepadKeybind, InputConsts.InputControlSchemes.GAMEPAD);

			inputCommandData.TrySetAsAxis(rebindableId, primaryName, secondaryName);

			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			Rebindables.Add((rebindableId, name, tooltip));

			return commandType;
		}

		public InputConsts.InputCommandType RegisterComposite(string name, string primaryName, string secondaryName)
		{
			_console.WriteLine($"RegisterComposite primary:{primaryName} secondary:{secondaryName}");

			var uniqueName = _manifest.UniqueName + name;

			var primaryId = (RebindableID)EnumUtils.Parse(typeof(RebindableID), _manifest.UniqueName + primaryName);
			var secondaryId = (RebindableID)EnumUtils.Parse(typeof(RebindableID), _manifest.UniqueName + secondaryName);

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Composite, commandType);
			inputCommandData.TrySetAsComposite(primaryId, _manifest.UniqueName + primaryName + "Primary", _manifest.UniqueName + primaryName + "Secondary", secondaryId, _manifest.UniqueName + secondaryName + "Primary", _manifest.UniqueName + secondaryName + "Secondary");
			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			return commandType;
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