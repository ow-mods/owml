﻿using System;
using System.Globalization;
using System.Linq;
using HarmonyLib;
using OWML.ModHelper.Menus.CustomInputs;
using UnityEngine;
using UnityEngine.UI;
using OWML.Common;
using OWML.Utils;
using Newtonsoft.Json.Linq;

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

			foreach (var (behaviour, modMenu) in MenuManager.ModSettingsMenus)
			{
				if (!modMenu.IsMenuEnabled())
				{
					continue;
				}

				var settings = behaviour.ModHelper.Config.Settings;
				var defaultSettings = behaviour.ModHelper.DefaultConfig;

				var options = modMenu.GetMenuOptions().Where(x => x.name != "UIElement-GammaButton").ToArray();

				for (var i = 0; i < options.Length; i++)
				{
					var menuOption = options[i];

					if (menuOption is OWMLOptionsSelectorElement selector)
					{
						var index = settings.Keys.ToList().IndexOf(selector.ModSettingKey);
						var settingObject = settings.Values.ElementAt(index) as JObject;
						var selectorOptions = settingObject["options"].ToArray().Select(x => x.ToString()).ToArray();
						var defaultValue = defaultSettings.GetSettingsValue<string>(selector.ModSettingKey);
						selector.Initialize(Array.IndexOf(selectorOptions, defaultValue), selectorOptions);
					}
					else if (menuOption is OWMLSliderElement slider)
					{
						var index = settings.Keys.ToList().IndexOf(slider.ModSettingKey);
						var settingObject = settings.Values.ElementAt(index) as JObject;
						var lower = settingObject["min"].ToObject<float>();
						var upper = settingObject["max"].ToObject<float>();
						slider.Initialize(defaultSettings.GetSettingsValue<float>(slider.ModSettingKey), lower, upper);
					}
					// twobutton is the same as toggle, just with an extra visual layer
					else if (menuOption is OWMLToggleElement toggle)
					{
						toggle.Initialize(defaultSettings.GetSettingsValue<bool>(toggle.ModSettingKey));
					}
					else if (menuOption is OWMLTextEntryElement textEntry)
					{
						if (textEntry.IsNumeric)
						{
							textEntry.SetCurrentValue(defaultSettings.GetSettingsValue<double>(textEntry.ModSettingKey).ToString(CultureInfo.InvariantCulture));
						}
						else
						{
							var defaultValue = defaultSettings.GetSettingsValue<string>(textEntry.ModSettingKey);
							textEntry.SetCurrentValue(defaultValue);
						}
					}
				}
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

		[HarmonyPostfix]
		[HarmonyPatch(typeof(TitleScreenManager), nameof(TitleScreenManager.FadeInTitleLogo))]
		public static void FadeInTitleLogo(TitleScreenManager __instance)
		{
			var owmlManifest = JsonHelper.LoadJsonObject<ModManifest>($"{Application.dataPath}/Managed/{Constants.OwmlManifestFileName}");
			__instance._gameVersionTextDisplay.text = $"Outer Wilds : {Application.version}{Environment.NewLine}OWML : {owmlManifest.Version}";
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(PopupMenu), nameof(PopupMenu.EnableMenu))]
		public static void EnableMenu(PopupMenu __instance)
		{
			var rootEnabled = __instance._menuActivationRoot.gameObject.activeSelf;

			// PopupCanvas is disabled after the menus are initialized, and overrideSorting can only be set when it's enabled
			__instance._menuActivationRoot.gameObject.SetActive(true);
			__instance._popupCanvas = __instance.gameObject.GetAddComponent<Canvas>();
			__instance._popupCanvas.overrideSorting = true;
			__instance._popupCanvas.sortingOrder = 30000;
			if (!rootEnabled)
			{
				__instance._menuActivationRoot.gameObject.SetActive(false);
			}
		}

		[HarmonyReversePatch]
		[HarmonyPatch(typeof(Menu), nameof(Menu.Activate))]
		public static void Menu_Activate_Stub(object instance) { }

		[HarmonyPrefix]
		[HarmonyPatch(typeof(TabbedMenu), nameof(TabbedMenu.Activate))]
		public static bool TabbedMenu_Activate(TabbedMenu __instance)
		{
			__instance.Initialize();
			var tabButton = __instance._firstSelectedTabButton;
			var selectable = MenuStackManager.SharedInstance.PeekSelectOnActivate();

			if (__instance._lastSelectableOnDeactivate != null)
			{
				Menu menu = null;

				foreach (var button in __instance._menuTabs)
				{
					var currentTabMenu = button.GetMenu();
					if (currentTabMenu is TabbedMenu currentTabbedMenu)
					{
						foreach (var subMenu in currentTabbedMenu._subMenus)
						{
							var menuOptions = subMenu.GetMenuOptions();
							foreach (var option in menuOptions)
							{
								if (option.GetSelectable() != __instance._lastSelectableOnDeactivate)
								{
									continue;
								}

								tabButton = button;
								menu = subMenu;
								selectable = __instance._lastSelectableOnDeactivate;
							}
						}
					}
					else
					{
						var menuOptions = currentTabMenu.GetMenuOptions();

						foreach (var option in menuOptions)
						{
							if (option.GetSelectable() != __instance._lastSelectableOnDeactivate)
							{
								continue;
							}

							tabButton = button;
							menu = button.GetMenu();
							selectable = __instance._lastSelectableOnDeactivate;
							break;
						}
					}

					if (menu != null)
					{
						break;
					}
				}
			}

			if (selectable != __instance._lastSelectableOnDeactivate)
			{
				for (int k = 0; k < __instance._tabSelectablePairs.Length; k++)
				{
					if (selectable == __instance._tabSelectablePairs[k].selectable)
					{
						tabButton = __instance._tabSelectablePairs[k].tabButton;
						break;
					}
				}
			}

			__instance.SelectTabButton(tabButton);
			Menu_Activate_Stub(__instance);
			Locator.GetMenuInputModule().OnInputModuleTab += __instance.OnInputModuleTabEvent;

			return false;
		}
	}
}
