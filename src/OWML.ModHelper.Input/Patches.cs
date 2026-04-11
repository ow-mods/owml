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
	}
}