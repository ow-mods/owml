using OWML.Common;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;

namespace OWML.ModHelper.Input
{
	public class RebindingHelper : IRebindingHelper
	{
		public static InputActionMap CustomActionMap = new("OWMLCustomActionMap");

		public List<RebindableID> Rebindables { get; } = new List<RebindableID>();

		private readonly IModConsole _console;

		public RebindingHelper(IModConsole console, IHarmonyHelper harmony)
		{
			_console = console;

			// this adds the prefix once per mod, but it shouldn't affect anything?
			harmony.AddPrefix(typeof(InputCommandManager).GetMethod("LoadActions", new Type[] { typeof(string) }), typeof(Patches), nameof(Patches.LoadActions));

			//harmony.AddPrefix(typeof(InputCommandUtils).GetMethod("TryCreateInputCommands", BindingFlags.Static | BindingFlags.Public), typeof(Patches), nameof(Patches.TryCreateInputCommands));
			//harmony.AddPrefix(typeof(InputCommandUtils).GetMethod("TryCreateBasicAction", BindingFlags.Static | BindingFlags.NonPublic), typeof(Patches), nameof(Patches.TryCreateBasicAction));
			//harmony.AddPrefix(typeof(InputCommandUtils).GetMethod("TryCreateAxisAction", BindingFlags.Static | BindingFlags.NonPublic), typeof(Patches), nameof(Patches.TryCreateAxisAction));
		}

		public IInputCommands GetCommand(InputConsts.InputCommandType commandType) => InputLibrary.GetInputCommand(commandType);

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

		public InputConsts.InputCommandType RegisterRebindableAxis(string name, string primary, string secondary)
		{
			_console.WriteLine($"RegisterRebindableAxis name:{name} primary:{primary} secondary:{secondary}");

			var commandType = EnumUtils.Create<InputConsts.InputCommandType>(name);
			var rebindableId = EnumUtils.Create<RebindableID>(primary);

			var inputCommandData = InputCommandDefinitions.InputCommandData.CreateCommandData(CommandDataType.Axis, commandType);
			inputCommandData.TrySetAsAxis(rebindableId, primary, secondary);
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
