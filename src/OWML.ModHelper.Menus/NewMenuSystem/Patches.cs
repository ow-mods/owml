using HarmonyLib;
using OWML.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	public static class Patches
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(TabButton), nameof(TabButton.InitializeNavigation))]
		public static bool InitializeNavigation(TabButton __instance)
		{
			if (__instance._mySelectable == null)
			{
				__instance._mySelectable = __instance.GetRequiredComponent<Button>();
			}

			var selectOnActivate = __instance._tabbedMenu.GetSelectOnActivate();
			if (__instance._firstSelectable != selectOnActivate)
			{
				__instance._firstSelectable = selectOnActivate;
				__instance._navigationInitialized = false;
			}

			if (!__instance._navigationInitialized)
			{
				__instance._navigationInitialized = true;

				var selectOnActivate2 = __instance._tabbedMenu.GetSelectOnActivate();

				if (selectOnActivate2 == null)
				{
					Debug.LogError($"{__instance._tabbedMenu.name} .GetSelectOnActivate() is null!", __instance);
				}

				var navigation = __instance._mySelectable.navigation;
				navigation.selectOnDown = selectOnActivate2;
				__instance._mySelectable.navigation = navigation;
			}

			return false;
		}
	}
}
