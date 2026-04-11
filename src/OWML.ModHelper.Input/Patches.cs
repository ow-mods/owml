using OWML.Logging;
using System;
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

		// GetActiveMenuOptions

		public static bool NotifyBindingChanged(SettingsMenuView __instance)
		{
			OWInput.NotifyBindingChanged();
			__instance._model.InitializeInputRebindables(__instance._listRebindableOptions);

			for (int i = 0; i < OWMLRebinding.ListCustomRebindableOptions.Count; i++)
			{
				OWMLRebinding.ListCustomRebindableOptions[i].Initialize(__instance._model);
			}

			return false;
		}

		public static bool OnButtonImagesChanged(SettingsMenuView __instance)
		{
			for (int i = 0; i < __instance._listRebindableOptions.Length; i++)
			{
				__instance._listRebindableOptions[i].UpdateDisplay(true);
			}

			for (int i = 0; i < OWMLRebinding.ListCustomRebindableOptions.Count; i++)
			{
				OWMLRebinding.ListCustomRebindableOptions[i].UpdateDisplay(true);
			}

			return false;
		}

		public static bool OnSettingsMenuPush(SettingsMenuView __instance, Menu menu)
		{
			if (menu == __instance._mainSettingsMenu)
			{
				__instance.Initialize();
				__instance._model.Initialize();
				__instance._model.InitializeInputRebindables(__instance._listRebindableOptions);

				for (int i = 0; i < OWMLRebinding.ListCustomRebindableOptions.Count; i++)
				{
					OWMLRebinding.ListCustomRebindableOptions[i].Initialize(__instance._model);
				}
			}

			return false;
		}

		public static bool RefreshRebindingDisplay(SettingsMenuView __instance, RebindableID rebindableId)
		{
			for (int i = 0; i < __instance._listRebindableOptions.Length; i++)
			{
				if (__instance._listRebindableOptions[i].GetRebindableID() == rebindableId)
				{
					__instance._listRebindableOptions[i].UpdateDisplay(false);
				}
			}

			for (int i = 0; i < OWMLRebinding.ListCustomRebindableOptions.Count; i++)
			{
				if (OWMLRebinding.ListCustomRebindableOptions[i].GetRebindableID() == rebindableId)
				{
					OWMLRebinding.ListCustomRebindableOptions[i].UpdateDisplay(false);
				}
			}

			return false;
		}

		public static bool UpdateKeyRebindingElementDisplays(SettingsMenuView __instance)
		{
			for (int i = 0; i < __instance._listRebindableOptions.Length; i++)
			{
				__instance._listRebindableOptions[i].UpdateDisplay(false);
			}

			for (int i = 0; i < OWMLRebinding.ListCustomRebindableOptions.Count; i++)
			{
				OWMLRebinding.ListCustomRebindableOptions[i].UpdateDisplay(false);
			}

			ISingleInputCommand singleInputCommand;
			ISingleAction singleAction;

			if ((singleInputCommand = InputLibrary.menuConfirm as ISingleInputCommand) == null || !singleInputCommand.TryCastAction<ISingleAction>(out singleAction))
			{
				return false;
			}

			ReadOnlyArray<global::UnityEngine.InputSystem.InputBinding> bindings = singleAction.Action.bindings;
			bool flag = true;
			foreach (global::UnityEngine.InputSystem.InputBinding inputBinding in bindings)
			{
				if (inputBinding.path.StartsWith("<Gamepad>"))
				{
					if (inputBinding.path.EndsWith("buttonSouth"))
					{
						flag = true;
						break;
					}
					flag = false;
					break;
				}
			}
			__instance._confirmToggleOption.Initialize(flag);
			return false;
		}

		public static bool ApplyBinding(RebindingState __instance, InputDevice device, string controlPath, bool usingGamepad)
		{
			string text = "";
			if (device is Gamepad)
			{
				text = "<Gamepad>";
			}
			else if (device is Mouse)
			{
				text = "<Mouse>";
			}
			else if (device is Keyboard)
			{
				text = "<Keyboard>";
			}

			ModConsole.OwmlConsole.WriteLine($"_negativeAction: {__instance._negativeAction}");
			ModConsole.OwmlConsole.WriteLine($"_positiveAction: {__instance._positiveAction}");

			InputAction inputAction;
			InputAction inputAction2;
			if (__instance._actionsRebound == 0)
			{
				if (__instance._isXAxisRebindable)
				{
					inputAction = __instance._negativeAction;
					inputAction2 = __instance._positiveAction;
				}
				else
				{
					inputAction = __instance._positiveAction;
					inputAction2 = __instance._negativeAction;
				}
			}
			else if (__instance._isXAxisRebindable)
			{
				inputAction = __instance._positiveAction;
				inputAction2 = __instance._negativeAction;
			}
			else
			{
				inputAction = __instance._negativeAction;
				inputAction2 = __instance._positiveAction;
			}

			ModConsole.OwmlConsole.WriteLine($"inputAction: {inputAction}");
			ModConsole.OwmlConsole.WriteLine($"inputAction2: {inputAction2}");

			int num = -1;
			ModConsole.OwmlConsole.WriteLine($"inputAction.bindings.Count: {inputAction.bindings.Count}");
			for (int i = 0; i < inputAction.bindings.Count; i++)
			{
				if (inputAction.bindings[i].path.StartsWith("<Gamepad>") && usingGamepad)
				{
					num = i;
					break;
				}
				if ((inputAction.bindings[i].path.StartsWith("<Mouse>") || inputAction.bindings[i].path.StartsWith("<Keyboard>")) && !usingGamepad)
				{
					num = i;
					break;
				}
			}
			int num2 = controlPath.IndexOf('/', 1);
			string text2 = text + controlPath.Substring(num2);
			string effectivePath = inputAction.bindings[num].effectivePath;
			inputAction.ChangeBinding(num).WithPath(text2);
			if (__instance._rebindableActionSingle != null)
			{
				RebindingUtil.ResolveConflicts(ref __instance._rebindableActionSingle, effectivePath, text2, num, usingGamepad, false);
			}
			__instance._actionsRebound++;
			bool flag = false;
			if (inputAction2 == null && __instance._actionsRebound == 1)
			{
				flag = true;
				__instance._rebindableActionSingle.SetAction(inputAction);
			}
			else if (inputAction2 != null && __instance._actionsRebound == 2)
			{
				flag = true;
				__instance._rebindableActionPair.SetAction(__instance._positiveAction, __instance._negativeAction);
			}
			Action<string, string, int, bool> onBindingApplied = __instance.OnBindingApplied;
			if (onBindingApplied != null)
			{
				onBindingApplied(effectivePath, text2, num, usingGamepad);
			}
			if (flag)
			{
				Action onFinishedRebinding = __instance.OnFinishedRebinding;
				if (onFinishedRebinding != null)
				{
					onFinishedRebinding();
				}
				InputSystem.onAfterUpdate -= __instance.OnInputEvent;
				__instance._validState = false;
				__instance._settingsView.UnregisterRebindingState(__instance);
				ModConsole.OwmlConsole.WriteLine("Input Done, Rebinding No longer Valid");
				return false;
			}
			if (inputAction2 != null && RebindingState._inputPairs.ContainsKey(text2))
			{
				__instance.ApplyBinding(device, RebindingState._inputPairs[text2], usingGamepad);
				return false;
			}
			__instance._rebindingStartTime = Time.realtimeSinceStartup;
			__instance._firstBindingUsingGamepad = usingGamepad;
			return false;
		}
	}
}