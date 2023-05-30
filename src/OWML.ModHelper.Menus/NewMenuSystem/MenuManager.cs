using HarmonyLib;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.ModHelper.Menus.NewMenuSystem.Interfaces;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	public class MenuManager : IMenuManager
	{
		private readonly IModConsole _console;
		private readonly IOwmlConfig _owmlConfig;
		private readonly IModUnityEvents _unityEvents;

		public ITitleMenuManager TitleMenuManager { get; private set; }
		public IPauseMenuManager PauseMenuManager { get; private set; }
		public IOptionsMenuManager OptionsMenuManager { get; private set; }

		public MenuManager(
			IModConsole console,
			IHarmonyHelper harmony,
			IModUnityEvents unityEvents,
			IOwmlConfig owmlConfig)
		{
			_console = console;
			_owmlConfig = owmlConfig;
			_unityEvents = unityEvents;
			TitleMenuManager = new TitleMenuManager();
			OptionsMenuManager = new OptionsMenuManager(console, unityEvents);

			var harmonyInstance = harmony.GetValue<Harmony>("_harmony");
			harmonyInstance.PatchAll(typeof(Patches));
		}

		public void CreateOWMLMenus(IList<IModBehaviour> modList)
		{
			void SaveConfig()
			{
				JsonHelper.SaveJsonObject($"{_owmlConfig.OWMLPath}{Constants.OwmlConfigFileName}", _owmlConfig);
			}

			EditExistingMenus();

			// Create menus and submenus
			var (modsMenu, modsMenuButton) = OptionsMenuManager.CreateTabWithSubTabs("MODS");
			var (owmlSubTab, owmlSubTabButton) = OptionsMenuManager.AddSubTab(modsMenu, "OWML");
			var (modsSubTab, modsSubTabButton) = OptionsMenuManager.AddSubTab(modsMenu, "MODS");

			// Create button on title screen that opens the mods menu
			var modsButton = TitleMenuManager.CreateTitleButton("MODS", 1, false);
			modsButton.OnSubmitAction += () => OptionsMenuManager.OpenOptionsAtTab(modsMenuButton);

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

			// Create buttons for each mod
			foreach (var mod in modList)
			{
				var button = OptionsMenuManager.CreateButton(modsSubTab, mod.ModHelper.Manifest.Name, "", MenuSide.CENTER);
				button.OnSubmitAction += () =>
				{
					var (newModTab, newModTabButton) = OptionsMenuManager.CreateStandardTab("MOD OPTIONS");

					var returnButton = OptionsMenuManager.CreateButton(newModTab, "Return", "Return to the mod selection list.", MenuSide.CENTER);
					returnButton.OnSubmitAction += () =>
					{
						OptionsMenuManager.OpenOptionsAtTab(modsMenuButton);

						// Give time for the modsMenu to activate before switching tabs
						_unityEvents.FireInNUpdates(() => modsMenu.SelectTabButton(modsSubTabButton), 2);
					};

					newModTab.OnDeactivateMenu += () =>
					{
						OptionsMenuManager.RemoveTab(newModTab);
					};

					foreach (var (name, setting) in mod.ModHelper.Config.Settings)
					{
						var configPath = $"{mod.ModHelper.Manifest.ModFolderPath}{Constants.ModConfigFileName}";

						var settingType = GetSettingType(setting);
						var label = name;
						var tooltip = "";

						var settingObject = setting as JObject;

						if (settingObject != default(JObject))
						{
							if (settingObject["title"] != null)
							{
								label = settingObject["title"].ToString();
							}

							if (settingObject["tooltip"] != null)
							{
								tooltip = settingObject["tooltip"].ToString();
							}
						}

						switch (settingType)
						{
							case "checkbox":
								var currentCheckboxValue = mod.ModHelper.Config.GetSettingsValue<bool>(name);
								var settingCheckbox = OptionsMenuManager.AddCheckboxInput(newModTab, label, tooltip, currentCheckboxValue);
								settingCheckbox.OnValueChanged += (bool newValue) =>
								{
									mod.ModHelper.Config.SetSettingsValue(name, newValue);
									JsonHelper.SaveJsonObject(configPath, mod.ModHelper.Config);
								};
								break;
							case "toggle":
								var currentToggleValue = mod.ModHelper.Config.GetSettingsValue<bool>(name);
								var yes = settingObject["yes"].ToString();
								var no = settingObject["no"].ToString();
								var settingToggle = OptionsMenuManager.AddToggleInput(newModTab, label, yes, no, tooltip, currentToggleValue);
								settingToggle.OnValueChanged += (bool newValue) =>
								{
									mod.ModHelper.Config.SetSettingsValue(name, newValue);
									JsonHelper.SaveJsonObject(configPath, mod.ModHelper.Config);
								};
								break;
							case "slider":
								break;
							case "selector":
								var currentSelectorValue = mod.ModHelper.Config.GetSettingsValue<string>(name);
								var options = settingObject["options"].ToArray().Select(x => x.ToString()).ToArray();
								var currentSelectedIndex = Array.IndexOf(options, currentSelectorValue);
								var settingSelector = OptionsMenuManager.AddSelectorInput(newModTab, label, options, tooltip, true, currentSelectedIndex);
								settingSelector.OnValueChanged += (int newIndex, string newSelection) =>
								{
									mod.ModHelper.Config.SetSettingsValue(name, newSelection);
									JsonHelper.SaveJsonObject(configPath, mod.ModHelper.Config);
								};
								break;
							case "separator":
								OptionsMenuManager.AddSeparator(newModTab, false);
								break;
							default:
								break;
						}
					}

					foreach (var item in mod.ModHelper.RebindingHelper.Rebindables)
					{
						_console.WriteLine($"KEY REBINDING ELEMENT - {item}");
						OptionsMenuManager.CreateRebinding(newModTab, "test", "test", item);
					}

					OptionsMenuManager.OpenOptionsAtTab(newModTabButton);
				};
			}
		}

		// This is to prevent the "AUDIO & LANGUAGE" tab text from overflowing it's boundaries when more tabs are added
		private void EditExistingMenus()
		{
			var optionsMenu = GameObject.Find("TitleMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>();
			foreach (var item in optionsMenu._menuTabs)
			{
				var text = item.GetComponent<UIStyleApplier>()._textItems[0];
				text.horizontalOverflow = HorizontalWrapMode.Wrap;
			}
		}

		private string GetSettingType(object setting)
		{
			var settingObject = setting as JObject;

			if (setting is bool || (settingObject != null && settingObject["type"].ToString() == "toggle" && (settingObject["yes"] == null || settingObject["no"] == null)))
			{
				return "checkbox";
			}
			else if (setting is string || (settingObject != null && settingObject["type"].ToString() == "text"))
			{
				return "text";
			}
			else if (setting is int || (settingObject != null && settingObject["type"].ToString() == "number"))
			{
				return "number";
			}
			else if (settingObject != null && settingObject["type"].ToString() == "toggle")
			{
				return "toggle";
			}
			else if (settingObject != null && settingObject["type"].ToString() == "selector")
			{
				return "selector";
			}
			else if (settingObject != null && settingObject["type"].ToString() == "slider")
			{
				return "slider";
			}
			else if (settingObject != null && settingObject["type"].ToString() == "separator")
			{
				return "separator";
			}

			return "unknown";
		}
	}
}