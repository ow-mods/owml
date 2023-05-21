using OWML.Common;
using OWML.Utils;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace OWML.ModHelper.Input
{
	public class RebindingHelper : IRebindingHelper
	{
		public static InputActionMap CustomActionMap = new("OWMLCustomActionMap");

		public List<RebindableID> Rebindables { get; } = new List<RebindableID>();

		private readonly IModConsole _console;

		public RebindingHelper(IModConsole console)
		{
			_console = console;
		}

		public IInputCommands GetCommand(InputConsts.InputCommandType commandType) => InputLibrary.GetInputCommand(commandType);

		// must be called in Start()
		public InputConsts.InputCommandType RegisterRebindableAxis(string name, string primary)
		{
			_console.WriteLine($"RegisterRebindableAxis name:{name} primary:{primary}");

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(name);
			var rebindableId = EnumUtils.Create<RebindableID>(primary);

			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);
			inputCommandData.TrySetAsAxis(rebindableId, primary);
			InputCommandDefinitions.AddInputCommandData(inputCommandData);

			Rebindables.Add(rebindableId);

			return commandType;
		}

		public InputAction RegisterCustomAction(string name)
			=> CustomActionMap.AddAction(
				name,
				InputActionType.Button,
				interactions: "OWInput",
				processors: "OWAxis",
				expectedControlLayout: "Button");

		public void AddBinding(InputAction action, string control)
			=> CustomActionMap.AddBinding(control, action, groups: "KeyboardMouse");
	}
}
