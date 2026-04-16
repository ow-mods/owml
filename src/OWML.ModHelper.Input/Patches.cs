using OWML.Logging;
using System;
using OWML.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

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

				foreach (var (uniqueName, actionMap) in OWMLRebinding.CustomActionMaps)
				{
					var existingActionMap = inputActionAsset.FindActionMap(actionMap.name);
					if (existingActionMap != null)
					{
						// actionmap already exists in game files, but mod could have updated to add new actions...

						foreach (var newAction in actionMap.actions)
						{
							if (existingActionMap.FindAction(newAction.name) == null)
							{
								var action = existingActionMap.AddAction(
									newAction.name,
									newAction.type,
									null,
									newAction.interactions,
									newAction.processors, 
									null,
									newAction.expectedControlType);

								foreach (var binding in newAction.bindings)
								{
									existingActionMap.AddBinding(binding.path, action, groups: binding.groups);
								}
							}
						}
					}
					else
					{
						inputActionAsset.AddActionMap(actionMap);
					}
				}

				flag = __instance.LoadActions(inputActionAsset);

				PlayerData.SaveInputSettings();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			__result = flag;
			return false;
		}

		public static bool TryCreateInputCommands(
			InputCommandDefinitions.InputCommandData data,
			InputActionAsset asset,
			out IInputCommands command,
			ref bool __result)
		{
			ModConsole.OwmlConsole.WriteLine($"TryCreateInputCommands Primary:{data.Primary?.ActionName1},{data.Primary?.ActionName2} Secondary:{data.Secondary?.ActionName1},{data.Secondary?.ActionName2}");

			command = null;
			if (!data.IsValid)
			{
				ModConsole.OwmlConsole.WriteLine("Invalid InputCommandData", MessageType.Error);
				//return false;
				__result = false;
				return false;
			}

			if (data.DataType == CommandDataType.Undefined)
			{
				//return false;
				__result = false;
				return false;
			}

			IVectorInputAction vectorInputAction;
			if (data.DataType == CommandDataType.Basic && TryCreateBasicAction(data.DataType, data.Primary, asset, out vectorInputAction))
			{
				command = new InputCommands(data.CommandType, vectorInputAction);
				//return true;
				__result = true;
				return false;
			}

			IAxisInputAction axisInputAction;
			if (!TryCreateAxisAction(data.DataType, data.Primary, asset, out axisInputAction))
			{
				ModConsole.OwmlConsole.WriteLine("Invalid primary axis ActionData for InputCommandType " + data.CommandType.ToString(), MessageType.Error);
				//return false;
				__result = false;
				return false;
			}

			if (data.IsComposite)
			{
				IAxisInputAction axisInputAction2;
				if (!TryCreateAxisAction(data.DataType, data.Secondary, asset, out axisInputAction2))
				{
					ModConsole.OwmlConsole.WriteLine("Invalid secondary axis ActionData for InputCommandType " + data.CommandType.ToString(), MessageType.Error);
					//return false;
					__result = false;
					return false;
				}
				IInputActionPair inputActionPair;
				IInputActionPair inputActionPair2;
				if ((inputActionPair = axisInputAction as IInputActionPair) == null || (inputActionPair2 = axisInputAction2 as IInputActionPair) == null)
				{
					ModConsole.OwmlConsole.WriteLine("Composite bindings not InputActionPairs", MessageType.Error);
					//return false;
					__result = false;
					return false;
				}
				command = new CompositeInputCommands(data.CommandType, inputActionPair, inputActionPair2);
				//return true;
				__result = true;
				return false;
			}
			else
			{
				if (!data.IsSingle)
				{
					Debug.LogError("Single Action InputCommand with type " + data.CommandType.ToString() + " unhandled");
					//return false;
					__result = false;
					return false;
				}
				command = new InputAxisCommands(data.CommandType, axisInputAction);
				//return true;
				__result = true;
				return false;
			}
		}

		public static bool TryCreateBasicAction(
			CommandDataType type,
			InputCommandDefinitions.InputActionData actionData,
			InputActionAsset asset,
			out IVectorInputAction action)

		{
			ModConsole.OwmlConsole.WriteLine($"- TryCreateBasicAction type:{type}");

			action = null;
			if (type != CommandDataType.Basic)
			{
				ModConsole.OwmlConsole.WriteLine($"-- FAIL - NOT BASIC", MessageType.Error);
				return false;
			}
			if (!actionData.IsSingle)
			{
				ModConsole.OwmlConsole.WriteLine($"-- FAIL - NOT SINGLE", MessageType.Error);
				return false;
			}
			InputAction inputAction = asset.FindAction(actionData.ActionName1, false);
			if (inputAction == null)
			{
				ModConsole.OwmlConsole.WriteLine($"-- FAIL - COULDNT FIND actionData.ActionName1:{actionData.ActionName1}", MessageType.Error);
				return false;
			}
			InputCommandManager.SanitizeActionForPlatform(inputAction);
			bool isRebindable = actionData.IsRebindable;
			if (InputCommandManager.RebindableInputActionsMap.ContainsKey(actionData.ID))
			{
				action = InputCommandManager.RebindableInputActionsMap[actionData.ID] as RebindableInputAction;
				if (action == null)
				{
					ModConsole.OwmlConsole.WriteLine("IRebindableInputAction RebindableInputActionsMap mismatch when loading " + actionData.ID, MessageType.Error);
				}
			}
			else if (isRebindable)
			{
				action = new RebindableInputAction(actionData.ID, inputAction);
				InputCommandManager.RebindableInputActionsMap.Add(actionData.ID, (RebindableInputAction)action);
			}
			else
			{
				action = new BasicInputAction(inputAction);
			}
			return action != null;
		}

		private static bool TryCreateAxisAction(CommandDataType type, InputCommandDefinitions.InputActionData actionData, InputActionAsset asset, out IAxisInputAction action)
		{
			ModConsole.OwmlConsole.WriteLine($"- TryCreateAxisAction type:{type}");

			action = null;
			if (type != CommandDataType.Axis && type != CommandDataType.Composite)
			{
				ModConsole.OwmlConsole.WriteLine($"-- FAIL - type != AXIS && TYPE != COMPOSITE (type:{type})", MessageType.Error);
				return false;
			}
			InputAction inputAction = asset.FindAction(actionData.ActionName1, false);
			if (inputAction == null)
			{
				ModConsole.OwmlConsole.WriteLine($"-- FAIL - COULDNT FIND actionData.ActionName1:{actionData.ActionName1}", MessageType.Error);
				return false;
			}
			InputCommandManager.SanitizeActionForPlatform(inputAction);
			bool isRebindable = actionData.IsRebindable;
			if (!actionData.IsPair)
			{
				ModConsole.OwmlConsole.WriteLine($"-- is not pair");
				if (InputCommandManager.RebindableInputActionsMap.ContainsKey(actionData.ID))
				{
					ModConsole.OwmlConsole.WriteLine($"-- found existing RebindableAxisInputAction");
					action = InputCommandManager.RebindableInputActionsMap[actionData.ID] as RebindableAxisInputAction;
					if (action == null)
					{
						ModConsole.OwmlConsole.WriteLine("IRebindableInputAction RebindableInputActionsMap mismatch when loading " + actionData.ID, MessageType.Error);
					}
				}
				else if (isRebindable)
				{
					ModConsole.OwmlConsole.WriteLine($"-- new RebindableAxisInputAction");
					action = new RebindableAxisInputAction(actionData.ID, inputAction);
					InputCommandManager.RebindableInputActionsMap.Add(actionData.ID, (RebindableAxisInputAction)action);
				}
				else
				{
					ModConsole.OwmlConsole.WriteLine($"-- new AxisInputAction");
					action = new AxisInputAction(inputAction);
				}
				return action != null;
			}

			ModConsole.OwmlConsole.WriteLine($"-- is pair");

			InputAction inputAction2 = asset.FindAction(actionData.ActionName2, false);
			if (inputAction2 == null)
			{
				ModConsole.OwmlConsole.WriteLine($"-- FAIL - COULDNT FIND actionData.ActionName2:{actionData.ActionName2}", MessageType.Error);
				return false;
			}

			InputCommandManager.SanitizeActionForPlatform(inputAction2);
			if (InputCommandManager.RebindableInputActionsMap.ContainsKey(actionData.ID))
			{
				ModConsole.OwmlConsole.WriteLine($"-- found existing RebindableInputActionPair");
				action = InputCommandManager.RebindableInputActionsMap[actionData.ID] as RebindableInputActionPair;
				if (action == null)
				{
					ModConsole.OwmlConsole.WriteLine("IRebindableInputAction RebindableInputActionsMap mismatch when loading " + actionData.ID, MessageType.Error);
				}
			}
			else if (isRebindable)
			{
				ModConsole.OwmlConsole.WriteLine($"-- new RebindableInputActionPair");
				action = new RebindableInputActionPair(actionData.ID, inputAction, inputAction2);
				InputCommandManager.RebindableInputActionsMap.Add(actionData.ID, (RebindableInputActionPair)action);
			}
			else
			{
				ModConsole.OwmlConsole.WriteLine($"-- new InputActionPair");
				action = new InputActionPair(inputAction, inputAction2);
			}
			return action != null;
		}
	}
}