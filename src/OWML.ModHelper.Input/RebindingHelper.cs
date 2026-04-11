using OWML.Common;
using OWML.Utils;
using System.Collections.Generic;
using UnityEngine.InputSystem;

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
			string primaryKeybind,
			string secondaryKeybind = null,
			string tooltip = null)
		{
			var uniqueName = _manifest.UniqueName + name;

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(uniqueName);
			var rebindableId = EnumUtils.Create<RebindableID>(uniqueName);
			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);

			if (string.IsNullOrEmpty(secondaryKeybind))
			{
				var primaryName = uniqueName + "Primary";
				var primaryAction = AddAction(_manifest, primaryName);
				AddBinding(_manifest, primaryAction, primaryKeybind, primaryName);

				inputCommandData.TrySetAsAxis(rebindableId, primaryName);
			}
			else
			{
				var primaryName = uniqueName + "Primary";
				var secondaryName = uniqueName + "Secondary";
				var primaryAction = AddAction(_manifest, primaryName);
				var secondaryAction = AddAction(_manifest, secondaryName);
				AddBinding(_manifest, primaryAction, primaryKeybind, primaryName);
				AddBinding(_manifest, secondaryAction, secondaryKeybind, secondaryName);

				inputCommandData.TrySetAsAxis(rebindableId, primaryName, secondaryName);
			}

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

		private void AddBinding(IModManifest manifest, InputAction action, string control, string name)
		{
			// AddBinding is always called after AddAction, so no need to validate the dict
			var syntax = OWMLRebinding.CustomActionMaps[manifest.UniqueName].AddBinding(control, action, groups: "KeyboardMouse");
			syntax.WithName(name);
		}
	}
}