using HarmonyLib;
using OWML.ModHelper.Menus.CustomInputs;
using UnityEngine;
using UnityEngine.UI;
using OWML.Common;
using OWML.Utils;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	public static class Patches
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(SettingsMenuView), nameof(SettingsMenuView.ResetToDefaultSettings))]
		public static void ResetToDefaultSettings(SettingsMenuView __instance)
		{
			if (MenuManager.OWMLSettingsMenu.IsMenuEnabled())
			{
				var owmlDefaultConfig = JsonHelper.LoadJsonObject<OwmlConfig>($"{Application.dataPath}/Managed/{Constants.OwmlDefaultConfigFileName}");

				var debugMode = MenuManager.OWMLSettingsMenu.GetMenuOptions()[0] as OWMLToggleElement;
				debugMode.Initialize(owmlDefaultConfig.DebugMode);

				var forceExe = MenuManager.OWMLSettingsMenu.GetMenuOptions()[1] as OWMLToggleElement;
				forceExe.Initialize(owmlDefaultConfig.ForceExe);

				var incremental = MenuManager.OWMLSettingsMenu.GetMenuOptions()[2] as OWMLToggleElement;
				incremental.Initialize(owmlDefaultConfig.IncrementalGC);

				return;
			}

			// TODO : the rest of the mod menus
		}

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
