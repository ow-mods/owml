using HarmonyLib;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	public class MenuManager : IMenuManager
	{
		enum SettingType
		{
			NONE,
			CHECKBOX,
			TOGGLE,
			TEXT,
			NUMBER,
			SELECTOR,
			SLIDER,
			SEPARATOR
		}

		private readonly IModConsole _console;
		private readonly IOwmlConfig _owmlConfig;
		private readonly IModUnityEvents _unityEvents;

		public ITitleMenuManager TitleMenuManager { get; private set; }
		public IPauseMenuManager PauseMenuManager { get; private set; }
		public IOptionsMenuManager OptionsMenuManager { get; private set; }
		public IPopupMenuManager PopupMenuManager { get; private set; }
		IList<IModBehaviour> IMenuManager.ModList { get; set; }

		internal static Menu OWMLSettingsMenu;
		internal static List<(IModBehaviour behaviour, Menu modMenu)> ModSettingsMenus = new();

		private bool _hasSetupMenusThisScene = false;
		private bool _forceModOptionsOpen;

		public MenuManager(
			IModConsole console,
			IHarmonyHelper harmony,
			IModUnityEvents unityEvents,
			IOwmlConfig owmlConfig,
			IModUnityEvents modUnityEvents)
		{
			_console = console;
			_owmlConfig = owmlConfig;
			_unityEvents = unityEvents;
			TitleMenuManager = new TitleMenuManager();
			PopupMenuManager = new PopupMenuManager(console, harmony, this);
			OptionsMenuManager = new OptionsMenuManager(console, unityEvents, PopupMenuManager, this);
			PauseMenuManager = new PauseMenuManager(console);

			var harmonyInstance = harmony.GetValue<Harmony>("_harmony");
			harmonyInstance.PatchAll(typeof(Patches));

			LoadManager.OnStartSceneLoad += (oldScene, newScene) =>
			{
				foreach (var mod in ((IMenuManager)this).ModList)
				{
					if (oldScene == OWScene.TitleScreen)
					{
						mod.CleanupTitleMenu();
						mod.CleanupOptionsMenu();
					}
					else if (oldScene is OWScene.SolarSystem or OWScene.EyeOfTheUniverse)
					{
						mod.CleanupPauseMenu();
						mod.CleanupOptionsMenu();
					}
				}
			};

			LoadManager.OnCompleteSceneLoad += (_, newScene) =>
			{
				_hasSetupMenusThisScene = false;
				if (newScene is OWScene.SolarSystem or OWScene.EyeOfTheUniverse)
				{
					SetupMenus(((IMenuManager)this).ModList);
				}
			};
		}

		public void ForceModOptionsOpen(bool force)
		{
			_forceModOptionsOpen = force;
		}

		internal void SetupMenus(IList<IModBehaviour> modList)
		{
			if (_hasSetupMenusThisScene)
			{
				return;
			}

			_hasSetupMenusThisScene = true;

			void SaveConfig()
			{
				JsonHelper.SaveJsonObject($"{_owmlConfig.OWMLPath}{Constants.OwmlConfigFileName}", _owmlConfig);
			}

			EditExistingMenus();

			// Create menus and submenus
			var (modsMenu, modsMenuButton) = OptionsMenuManager.CreateTabWithSubTabs("MODS");
			var (modsSubTab, modsSubTabButton) = OptionsMenuManager.AddSubTab(modsMenu, "MODS");
			var (owmlSubTab, owmlSubTabButton) = OptionsMenuManager.AddSubTab(modsMenu, "OWML");

			OWMLSettingsMenu = owmlSubTab;

			owmlSubTab.OnActivateMenu += () =>
			{
				var settingsMenuView = UnityEngine.Object.FindObjectOfType<SettingsMenuView>();
				settingsMenuView._resetToDefaultsPrompt.SetText("Default Settings (OWML)");
				settingsMenuView._resetToDefaultButton.RefreshTextAndImages(false);
			};

			modsSubTab.OnActivateMenu += () =>
			{
				var settingsMenuView = UnityEngine.Object.FindObjectOfType<SettingsMenuView>();
				settingsMenuView._resetToDefaultButton.gameObject.SetActive(false);
			};

			modsSubTab.OnDeactivateMenu += () =>
			{
				var settingsMenuView = UnityEngine.Object.FindObjectOfType<SettingsMenuView>();
				settingsMenuView._resetToDefaultButton.gameObject.SetActive(true);
			};

			if (LoadManager.GetCurrentScene() == OWScene.TitleScreen)
			{
				// Create button on title screen that opens the mods menu
				var modsButton = TitleMenuManager.CreateTitleButton("MODS", 1, false);
				modsButton.OnSubmitAction += () => OptionsMenuManager.OpenOptionsAtTab(modsMenuButton);
			}
			else
			{
				// Create button on pause screen that opens the mods menu
				var modsButton = PauseMenuManager.MakeSimpleButton("MODS", 2, false);
				modsButton.OnSubmitAction += () => OptionsMenuManager.OpenOptionsAtTab(modsMenuButton);
			}

			#region OWML Settings

			var debugModeCheckbox = OptionsMenuManager.AddCheckboxInput(
				owmlSubTab,
				"Debug Mode",
				"Enables verbose logging.",
				_owmlConfig.DebugMode);
			debugModeCheckbox.OnValueChanged += (bool newValue) =>
			{
				_owmlConfig.DebugMode = newValue;
				SaveConfig();
			};

			var forceExeCheckbox = OptionsMenuManager.AddCheckboxInput(
				owmlSubTab,
				"Force EXE",
				"Forces OWML to run the .exe directly, instead of going through Steam or Epic.",
				_owmlConfig.ForceExe);
			forceExeCheckbox.OnValueChanged += (bool newValue) =>
			{
				_owmlConfig.ForceExe = newValue;
				SaveConfig();
			};

			var incrementalGCCheckbox = OptionsMenuManager.AddCheckboxInput(
				owmlSubTab,
				"Incremental GC",
				"Incremental GC (garbage collection) can help reduce lag with some mods.",
				_owmlConfig.IncrementalGC);
			incrementalGCCheckbox.OnValueChanged += (bool newValue) =>
			{
				_owmlConfig.IncrementalGC = newValue;
				SaveConfig();
			};

			#endregion

			var modsWithSettings = modList.Where(x => x.ModHelper.Config.Settings.Count != 0);
			var modsWithoutSettings = modList.Where(x => x.ModHelper.Config.Settings.Count == 0);

			// Create buttons for each mod
			foreach (var mod in modsWithSettings)
			{
				var button = OptionsMenuManager.CreateButton(modsSubTab, mod.ModHelper.Manifest.Name, "", MenuSide.CENTER);
				button.OnSubmitAction += () =>
				{
					var (newModTab, newModTabButton) = OptionsMenuManager.CreateStandardTab("MOD OPTIONS");

					ModSettingsMenus.Add((mod, newModTab));

					OptionsMenuManager.CreateLabel(newModTab, $"{mod.ModHelper.Manifest.Name} {mod.ModHelper.Manifest.Version} by {mod.ModHelper.Manifest.Author}");

					var returnButton = OptionsMenuManager.CreateButton(newModTab, "Return", "Return to the mod selection list.", MenuSide.CENTER);
					returnButton.OnSubmitAction += () =>
					{
						OptionsMenuManager.OpenOptionsAtTab(modsMenuButton);

						// Give time for the modsMenu to activate before switching tabs
						_unityEvents.FireInNUpdates(() => modsMenu.SelectTabButton(modsSubTabButton), 2);
					};

					newModTab.OnActivateMenu += () =>
					{
						var settingsMenuView = UnityEngine.Object.FindObjectOfType<SettingsMenuView>();
						settingsMenuView._resetToDefaultsPrompt.SetText($"Default Settings ({mod.ModHelper.Manifest.Name})");
						settingsMenuView._resetToDefaultButton.RefreshTextAndImages(false);
					};

					newModTab.OnDeactivateMenu += () =>
					{
						if (_forceModOptionsOpen)
						{
							return;
						}

						// Fixes tab dissapearing when you click on it again
						// Clicking on a tab closes and opens it again
						_unityEvents.FireOnNextUpdate(() =>
						{
							if (!newModTab._isActivated)
							{
								OptionsMenuManager.RemoveTab(newModTab);
							}
						});
					};

					foreach (var (name, setting) in mod.ModHelper.Config.Settings)
					{
						var settingType = GetSettingType(setting);
						var label = mod.ModHelper.MenuTranslations.GetLocalizedString(name);
						var tooltip = "";

						var settingObject = setting as JObject;

						if (settingObject != default(JObject))
						{
							if (settingObject["dlcOnly"]?.ToObject<bool>() ?? false)
							{
								if (EntitlementsManager.IsDlcOwned() == EntitlementsManager.AsyncOwnershipStatus.NotOwned)
								{
									continue;
								}
							}

							if (settingObject["title"] != null)
							{
								label = mod.ModHelper.MenuTranslations.GetLocalizedString(settingObject["title"].ToString());
							}

							if (settingObject["tooltip"] != null)
							{
								tooltip = mod.ModHelper.MenuTranslations.GetLocalizedString(settingObject["tooltip"].ToString());
							}
						}

						switch (settingType)
						{
							case SettingType.CHECKBOX:
								var currentCheckboxValue = mod.ModHelper.Config.GetSettingsValue<bool>(name);
								var settingCheckbox = OptionsMenuManager.AddCheckboxInput(newModTab, label, tooltip, currentCheckboxValue);
								settingCheckbox.ModSettingKey = name;
								settingCheckbox.OnValueChanged += (bool newValue) =>
								{
									mod.ModHelper.Config.SetSettingsValue(name, newValue);
									mod.ModHelper.Storage.Save(mod.ModHelper.Config, Constants.ModConfigFileName);
									mod.Configure(mod.ModHelper.Config);
								};
								break;
							case SettingType.TOGGLE:
								var currentToggleValue = mod.ModHelper.Config.GetSettingsValue<bool>(name);
								var yes = settingObject["yes"].ToString();
								var no = settingObject["no"].ToString();
								var settingToggle = OptionsMenuManager.AddToggleInput(newModTab, label, yes, no, tooltip, currentToggleValue);
								settingToggle.ModSettingKey = name;
								settingToggle.OnValueChanged += (bool newValue) =>
								{
									mod.ModHelper.Config.SetSettingsValue(name, newValue);
									mod.ModHelper.Storage.Save(mod.ModHelper.Config, Constants.ModConfigFileName);
									mod.Configure(mod.ModHelper.Config);
								};
								break;
							case SettingType.SELECTOR:
								var currentSelectorValue = mod.ModHelper.Config.GetSettingsValue<string>(name);
								var options = settingObject["options"].ToArray().Select(x => mod.ModHelper.MenuTranslations.GetLocalizedString(x.ToString())).ToArray();
								var currentSelectedIndex = Array.IndexOf(options, currentSelectorValue);
								var settingSelector = OptionsMenuManager.AddSelectorInput(newModTab, label, options, tooltip, true, currentSelectedIndex);
								settingSelector.ModSettingKey = name;
								settingSelector.OnValueChanged += (int newIndex, string newSelection) =>
								{
									mod.ModHelper.Config.SetSettingsValue(name, newSelection);
									mod.ModHelper.Storage.Save(mod.ModHelper.Config, Constants.ModConfigFileName);
									mod.Configure(mod.ModHelper.Config);
								};
								break;
							case SettingType.SEPARATOR:
								OptionsMenuManager.AddSeparator(newModTab, false);
								break;
							case SettingType.SLIDER:
								var currentSliderValue = mod.ModHelper.Config.GetSettingsValue<float>(name);
								var lower = settingObject["min"].ToObject<float>();
								var upper = settingObject["max"].ToObject<float>();
								var settingSlider = OptionsMenuManager.AddSliderInput(newModTab, label, lower, upper, tooltip, currentSliderValue);
								settingSlider.ModSettingKey = name;
								settingSlider.OnValueChanged += (float newValue) =>
								{
									mod.ModHelper.Config.SetSettingsValue(name, newValue);
									mod.ModHelper.Storage.Save(mod.ModHelper.Config, Constants.ModConfigFileName);
									mod.Configure(mod.ModHelper.Config);
								};
								break;
							case SettingType.TEXT:
								var currentTextValue = mod.ModHelper.Config.GetSettingsValue<string>(name);
								var textInput = OptionsMenuManager.AddTextEntryInput(newModTab, label, currentTextValue, tooltip, false);
								textInput.ModSettingKey = name;
								textInput.OnConfirmEntry += () =>
								{
									var newValue = textInput.GetInputText();
									mod.ModHelper.Config.SetSettingsValue(name, newValue);
									mod.ModHelper.Storage.Save(mod.ModHelper.Config, Constants.ModConfigFileName);
									mod.Configure(mod.ModHelper.Config);
									textInput.SetText(newValue);
								};
								break;
							case SettingType.NUMBER:
								var currentValue = mod.ModHelper.Config.GetSettingsValue<double>(name);
								var numberInput = OptionsMenuManager.AddTextEntryInput(newModTab, label, currentValue.ToString(CultureInfo.CurrentCulture), tooltip, true);
								numberInput.ModSettingKey = name;
								numberInput.OnConfirmEntry += () =>
								{
									var newValue = double.Parse(numberInput.GetInputText());
									mod.ModHelper.Config.SetSettingsValue(name, newValue);
									mod.ModHelper.Storage.Save(mod.ModHelper.Config, Constants.ModConfigFileName);
									mod.Configure(mod.ModHelper.Config);
									numberInput.SetText(newValue.ToString());
								};
								break;
							default:
								_console.WriteLine($"Couldn't generate input for unkown input type {settingType}", MessageType.Error);
								OptionsMenuManager.CreateLabel(newModTab, $"Unknown {settingType} : {name}");
								break;
						}
					}

					OptionsMenuManager.OpenOptionsAtTab(newModTabButton);
					Locator.GetMenuAudioController().PlayChangeTab();
				};
			}

			OptionsMenuManager.AddSeparator(modsSubTab, true);

			foreach (var mod in modsWithoutSettings)
			{
				OptionsMenuManager.CreateLabel(modsSubTab, mod.ModHelper.Manifest.Name);
			}

			foreach (var mod in modList)
			{
				try
				{
					if (LoadManager.GetCurrentScene() == OWScene.TitleScreen)
					{
						mod.SetupTitleMenu(mod.ModHelper.MenuHelper.TitleMenuManager);
					}
					else if (LoadManager.GetCurrentScene() is OWScene.SolarSystem or OWScene.EyeOfTheUniverse)
					{
						mod.SetupPauseMenu(mod.ModHelper.MenuHelper.PauseMenuManager);
					}

					mod.SetupOptionsMenu(mod.ModHelper.MenuHelper.OptionsMenuManager);
				}
				catch (Exception ex)
				{
					_console.WriteLine($"Exception when setting up menus for {mod.ModHelper.Manifest.UniqueName} : {ex}", MessageType.Error);
				}
			}
		}

		// This is to prevent the "AUDIO & LANGUAGE" tab text from overflowing its boundaries when more tabs are added
		private void EditExistingMenus()
		{
			TabbedMenu optionsMenu;

			if (LoadManager.GetCurrentScene() == OWScene.TitleScreen)
			{
				optionsMenu = GameObject.Find("TitleMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>();

				var animController = GameObject.Find("TitleMenuManagers").GetComponent<TitleAnimationController>();
				foreach (var item in animController._buttonFadeControllers)
				{
					var layoutElement = item.group.gameObject.GetComponent<LayoutElement>();
					layoutElement.minHeight = 28; // seems to be the minimum
				}
			}
			else
			{
				optionsMenu = GameObject.Find("PauseMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>();
			}

			foreach (var item in optionsMenu._menuTabs)
			{
				var text = item.GetComponent<UIStyleApplier>()._textItems[0];
				text.horizontalOverflow = HorizontalWrapMode.Wrap;
				
				// Give a little bit of margin to account for the dividing lines
				// (as otherwise it can look like the text is on top of them when in mouse mode)
				float margin = 0.03f;
				text.rectTransform.anchorMin = new Vector2(margin, 0.0f);
				text.rectTransform.anchorMax = new Vector2(1f - margin, 1.0f);
			}
		}

		private SettingType GetSettingType(object setting)
		{
			var settingObject = setting as JObject;

			if (setting is bool || (settingObject != null && settingObject["type"].ToString() == "toggle" && (settingObject["yes"] == null || settingObject["no"] == null)))
			{
				return SettingType.CHECKBOX;
			}
			else if (setting is string || (settingObject != null && settingObject["type"].ToString() == "text"))
			{
				return SettingType.TEXT;
			}
			else if (setting is int || setting is long || setting is float || setting is double || setting is decimal || (settingObject != null && settingObject["type"].ToString() == "number"))
			{
				return SettingType.NUMBER;
			}
			else if (settingObject != null && settingObject["type"].ToString() == "toggle")
			{
				return SettingType.TOGGLE;
			}
			else if (settingObject != null && settingObject["type"].ToString() == "selector")
			{
				return SettingType.SELECTOR;
			}
			else if (settingObject != null && settingObject["type"].ToString() == "slider")
			{
				return SettingType.SLIDER;
			}
			else if (settingObject != null && settingObject["type"].ToString() == "separator")
			{
				return SettingType.SEPARATOR;
			}

			_console.WriteLine($"Couldn't work out setting type. Type:{setting.GetType().Name} SettingObjectType:{settingObject?["type"].ToString()}", MessageType.Error);
			return SettingType.NONE;
		}
	}
}
