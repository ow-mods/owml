using OWML.Logging;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OWML.ModHelper.Input
{
	internal static class Patches
	{
		public static bool LoadActions(InputCommandManager __instance, string json, ref bool __result)
		{
			var flag = false;
			try
			{
				var inputActionAsset = InputActionAsset.FromJson(json);

				foreach (var action in RebindingHelper.CustomActionMap.actions)
				{
					ModConsole.OwmlConsole.WriteLine($"- name:{action.name}");
				}

				var existingActionMap = inputActionAsset.FindActionMap(RebindingHelper.CustomActionMap.name);
				if (existingActionMap != null)
				{
					ModConsole.OwmlConsole.WriteLine("Found existing custom action map");
					foreach (var newAction in RebindingHelper.CustomActionMap.actions)
					{
						if (existingActionMap.FindAction(newAction.name) == null)
						{
							ModConsole.OwmlConsole.WriteLine($"- Couldn't find {newAction.name} in existing action map");
							existingActionMap.AddAction(newAction.name, newAction.type, null, newAction.interactions, newAction.processors, null, newAction.expectedControlType);
							foreach (var binding in newAction.bindings)
							{
								existingActionMap.AddBinding(binding);
							}
						}
					}
				}
				else
				{
					ModConsole.OwmlConsole.WriteLine("Inserting custom action map");
					inputActionAsset.AddActionMap(RebindingHelper.CustomActionMap);
				}

				
				flag = __instance.LoadActions(inputActionAsset);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			__result = flag;
			return false;
		}

		public static bool TryCreateInputCommands(InputCommandDefinitions.InputCommandData data, InputActionAsset asset, out IInputCommands command, ref bool __result)
		{
			ModConsole.OwmlConsole.WriteLine($"TryCreateInputCommands - CommandType:{data.CommandType} DataType:{data.DataType}");

			command = null;
			if (!data.IsValid)
			{
				Debug.LogError("Invalid InputCommandData");
				__result = false;
				return false;
			}

			if (data.DataType == CommandDataType.Undefined)
			{
				__result = false;
				return false;
			}

			if (data.DataType == CommandDataType.Basic && InputCommandUtils.TryCreateBasicAction(data.DataType, data.Primary, asset, out var vectorInputAction))
			{
				command = new InputCommands(data.CommandType, vectorInputAction);
				__result = true;
				return false;
			}

			if (!InputCommandUtils.TryCreateAxisAction(data.DataType, data.Primary, asset, out var axisInputAction))
			{
				Debug.LogError("Invalid primary axis ActionData for InputCommandType " + data.CommandType.ToString());
				__result = false;
				return false;
			}

			if (data.IsComposite)
			{
				if (!InputCommandUtils.TryCreateAxisAction(data.DataType, data.Secondary, asset, out var axisInputAction2))
				{
					Debug.LogError("Invalid secondary axis ActionData for InputCommandType " + data.CommandType.ToString());
					__result = false;
					return false;
				}

				if (axisInputAction is not IInputActionPair inputActionPair || axisInputAction2 is not IInputActionPair inputActionPair2)
				{
					Debug.LogError("Composite bindings not InputActionPairs");
					__result = false;
					return false;
				}

				command = new CompositeInputCommands(data.CommandType, inputActionPair, inputActionPair2);
				__result = true;
				return false;
			}
			else
			{
				if (!data.IsSingle)
				{
					Debug.LogError("Single Action InputCommand with type " + data.CommandType.ToString() + " unhandled");
					__result = false;
					return false;
				}

				command = new InputAxisCommands(data.CommandType, axisInputAction);
				__result = true;
				return false;
			}
		}

		public static bool TryCreateBasicAction(CommandDataType type, InputCommandDefinitions.InputActionData actionData, InputActionAsset asset, out IVectorInputAction action, ref bool __result)
		{
			ModConsole.OwmlConsole.WriteLine($"TryCreateBasicAction - actionData.actionName1:{actionData.ActionName1}");

			action = null;
			if (type != CommandDataType.Basic)
			{
				ModConsole.OwmlConsole.WriteLine("- NOT BASIC!");
				__result = false;
				return false;
			}

			if (!actionData.IsSingle)
			{
				ModConsole.OwmlConsole.WriteLine("- NOT SINGLE!");
				__result = false;
				return false;
			}

			var inputAction = asset.FindAction(actionData.ActionName1, false);
			if (inputAction == null)
			{
				ModConsole.OwmlConsole.WriteLine("- NOT IN ASSET!");
				__result = false;
				return false;
			}

			InputCommandManager.SanitizeActionForPlatform(inputAction);
			var isRebindable = actionData.IsRebindable;
			if (InputCommandManager.RebindableInputActionsMap.ContainsKey(actionData.ID))
			{
				ModConsole.OwmlConsole.WriteLine($" - in rebindable action map");
				action = InputCommandManager.RebindableInputActionsMap[actionData.ID] as RebindableInputAction;
				if (action == null)
				{
					Debug.LogError("IRebindableInputAction RebindableInputActionsMap mismatch when loading " + actionData.ID);
				}
			}
			else if (isRebindable)
			{
				ModConsole.OwmlConsole.WriteLine($" - not in rebindable action map, is rebindable");
				action = new RebindableInputAction(actionData.ID, inputAction);
				InputCommandManager.RebindableInputActionsMap.Add(actionData.ID, (RebindableInputAction)action);
			}
			else
			{
				ModConsole.OwmlConsole.WriteLine($" - not in rebindable action map, not rebindable");
				action = new BasicInputAction(inputAction);
			}

			__result = action != null;
			return false;
		}

		public static bool TryCreateAxisAction(CommandDataType type, InputCommandDefinitions.InputActionData actionData, InputActionAsset asset, out IAxisInputAction action, ref bool __result)
		{
			ModConsole.OwmlConsole.WriteLine($"TryCreateAxisAction - actionData.actionName1:{actionData.ActionName1}");

			action = null;
			if (type != CommandDataType.Axis && type != CommandDataType.Composite)
			{
				ModConsole.OwmlConsole.WriteLine($"- type is not axis or composite!");
				__result = false;
				return false;
			}

			InputAction inputAction = asset.FindAction(actionData.ActionName1, false);
			if (inputAction == null)
			{
				ModConsole.OwmlConsole.WriteLine($"- NOT IN ASSET");
				__result = false;
				return false;
			}

			InputCommandManager.SanitizeActionForPlatform(inputAction);
			bool isRebindable = actionData.IsRebindable;
			if (!actionData.IsPair)
			{
				if (InputCommandManager.RebindableInputActionsMap.ContainsKey(actionData.ID))
				{
					action = InputCommandManager.RebindableInputActionsMap[actionData.ID] as RebindableAxisInputAction;
					if (action == null)
					{
						Debug.LogError("IRebindableInputAction RebindableInputActionsMap mismatch when loading " + actionData.ID);
					}
				}
				else if (isRebindable)
				{
					action = new RebindableAxisInputAction(actionData.ID, inputAction);
					InputCommandManager.RebindableInputActionsMap.Add(actionData.ID, (RebindableAxisInputAction)action);
				}
				else
				{
					action = new AxisInputAction(inputAction);
				}

				__result = action != null;
				return false;
			}

			InputAction inputAction2 = asset.FindAction(actionData.ActionName2, false);
			if (inputAction2 == null)
			{
				__result = false;
				return false;
			}

			InputCommandManager.SanitizeActionForPlatform(inputAction2);
			if (InputCommandManager.RebindableInputActionsMap.ContainsKey(actionData.ID))
			{
				action = InputCommandManager.RebindableInputActionsMap[actionData.ID] as RebindableInputActionPair;
				if (action == null)
				{
					Debug.LogError("IRebindableInputAction RebindableInputActionsMap mismatch when loading " + actionData.ID);
				}
			}
			else if (isRebindable)
			{
				action = new RebindableInputActionPair(actionData.ID, inputAction, inputAction2);
				InputCommandManager.RebindableInputActionsMap.Add(actionData.ID, (RebindableInputActionPair)action);
			}
			else
			{
				action = new InputActionPair(inputAction, inputAction2);
			}

			__result = action != null;
			return false;
		}
	}
}
