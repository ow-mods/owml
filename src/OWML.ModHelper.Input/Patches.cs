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
			ModConsole.OwmlConsole.WriteLine("Inserting custom action map...");

			var flag = false;
			try
			{
				var inputActionAsset = InputActionAsset.FromJson(json);

				foreach (var action in RebindingHelper.CustomActionMap.actions)
				{
					ModConsole.OwmlConsole.WriteLine($"- name:{action.name}");
				}

				inputActionAsset.AddActionMap(RebindingHelper.CustomActionMap);
				flag = __instance.LoadActions(inputActionAsset);
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
