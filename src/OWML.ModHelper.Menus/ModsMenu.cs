using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
	public class ModsMenu : ModPopupMenu, IModsMenu
	{
		private const string ModsTitle = "MODS";

		public IModConfigMenuBase OwmlMenu { get; }

		private readonly IModStorage _storage;
		private readonly List<IModConfigMenu> _modConfigMenus = new();
		private readonly List<(IModData data, IModBehaviour mod)> _noConfigMods = new();
		private readonly List<MenuOption> _menuOptions = new();
		private IModMenus _menus;

		public ModsMenu(
			IModConfigMenuBase owmlMenu,
			IModStorage storage,
			IModConsole console)
				: base(console)
		{
			OwmlMenu = owmlMenu;
			_storage = storage;
		}

		public void AddMod(IModData modData, IModBehaviour mod)
		{
			if (modData.Config.Settings.Count != 0)
			{
				_modConfigMenus.Add(new ModConfigMenu(modData, mod, _storage, Console));
			}
			else
			{
				_noConfigMods.Add((modData, mod));
			}
		}

		public IModConfigMenu GetModMenu(IModBehaviour modBehaviour)
		{
			Console.WriteLine("Registering " + modBehaviour.ModHelper.Manifest.UniqueName);
			var modConfigMenu = _modConfigMenus.FirstOrDefault(x => x.Mod == modBehaviour);
			if (modConfigMenu == null)
			{
				Console.WriteLine($"Error - {modBehaviour.ModHelper.Manifest.UniqueName} isn't added.", MessageType.Error);
				return null;
			}
			return modConfigMenu;
		}

		public void Initialize(IModMenus menus, IModOWMenu owMenu)
		{
			var modsToRemove = new List<(IModData data, IModBehaviour mod)>();

			foreach (var item in _noConfigMods)
			{
				if (item.mod.ModHelper.RebindingHelper.Rebindables.Count != 0)
				{
					modsToRemove.Add(item);
					_modConfigMenus.Add(new ModConfigMenu(item.data, item.mod, _storage, Console));
				}
			}

			_noConfigMods.RemoveAll(x => modsToRemove.Contains(x));

			_modConfigMenus.ForEach(x => x.RemoveAllListeners());
			OwmlMenu.RemoveAllListeners();

			_menus = menus;
			Menu = owMenu.Menu;

			var modsButton = owMenu.OptionsButton.Duplicate(ModsTitle);
			modsButton.Button.name = ModsTitle + "_button";
			var options = owMenu.OptionsMenu;

			var modsMenu = CreateModsMenu(options);
			modsButton.OnClick += modsMenu.Open;
			_modConfigMenus.ForEach(x => x.OnClosed += modsMenu.Open);
			OwmlMenu.OnClosed += modsMenu.Open;
		}

		private int CreateSeparator(IModTabbedMenu options, IModMenu menu, int index, string text)
		{
			var separator = new ModSeparator(menu)
			{
				Title = text
			};
			menu.AddSeparator(separator, index++);
			separator.Element.transform.localScale = options.GameplayTab.Buttons.First().Button.transform.localScale;
			return index;
		}

		private IModPopupMenu CreateModsMenu(IModTabbedMenu options)
		{
			_menuOptions.Clear();
			var modsTab = CreateTab(options, ModsTitle, true);

			var owmlButton = CreateButton(options, Constants.OwmlTitle);
			modsTab.AddButton((IModButtonBase)owmlButton, 0);
			var owmlTab = CreateTab(options, Constants.OwmlTitle, false);
			InitConfigMenu(OwmlMenu, options, owmlTab);
			owmlButton.OnClick += () => owmlTab.Open();

			var enabledMods = _modConfigMenus.Where(modConfigMenu => modConfigMenu.ModData.Config.Enabled).ToList();
			var index = CreateBlockOfButtons(options, modsTab, enabledMods, 1, "-- ENABLED MODS --");

			foreach (var (data, mod) in _noConfigMods.Where(x => x.data.Config.Enabled))
			{
				index = CreateSeparator(options, modsTab, index, data.Manifest.Name);
			}

			var disabledMods = _modConfigMenus.Except(enabledMods).ToList();
			index = CreateBlockOfButtons(options, modsTab, disabledMods, index, "-- DISABLED MODS --");

			foreach (var (data, mod) in _noConfigMods.Where(x => !x.data.Config.Enabled))
			{
				index = CreateSeparator(options, modsTab, index, data.Manifest.Name);
			}

			modsTab.Menu.SetValue("_menuOptions", _menuOptions.ToArray());
			return modsTab;
		}

		private int CreateBlockOfButtons(IModTabbedMenu options, IModTabMenu menu,
			List<IModConfigMenu> configMenus, int index, string title)
		{
			Console.WriteLine($"CreateBlockOfButtons - {configMenus.Count} mods");

			index = CreateSeparator(options, menu, index, $"{Environment.NewLine}{title}{Environment.NewLine}");

			if (configMenus.Count <= 0)
			{
				return index;
			}

			foreach (var modConfigMenu in configMenus)
			{
				var modButton = CreateButton(options, modConfigMenu.Manifest.Name);
				var modTab = CreateTab(options, modConfigMenu.Manifest.Name, false);
				InitConfigMenu(modConfigMenu, options, modTab);
				modButton.OnClick += () => modTab.Open();
				menu.AddButton((IModButtonBase)modButton, index++);
				InitRebindingOptions(modConfigMenu, options, modTab);
			}
			return index;
		}

		private void InitConfigMenu(IModConfigMenuBase modConfigMenu, IModTabbedMenu options, IModTabMenu modTabMenu)
		{
			var toggleTemplate = options.GraphicsTab.ToggleInputs[0];
			var sliderTemplate = options.GraphicsTab.SliderInputs.Find(sliderInput => sliderInput.HasValueText) ?? options.InputTab.SliderInputs[0];
			var selectorTemplate = options.GraphicsTab.SelectorInputs.Find(selectorInput => selectorInput.SelectorElement && selectorInput.SelectorElement.ShouldEnable());
			var textInputTemplate = new ModTextInput(selectorTemplate.Copy().SelectorElement, modConfigMenu, _menus.PopupManager);
			textInputTemplate.Hide();
			var numberInputTemplate = new ModNumberInput(selectorTemplate.Copy().SelectorElement, modConfigMenu, _menus.PopupManager);
			numberInputTemplate.Hide();
			var seperatorTemplate = new ModSeparator(modConfigMenu);
			modConfigMenu.Initialize(modTabMenu.Menu, toggleTemplate, sliderTemplate, textInputTemplate, numberInputTemplate, selectorTemplate, seperatorTemplate);
			modConfigMenu.UpdateUIValues();
		}

		private void InitRebindingOptions(IModConfigMenu modConfigMenu, IModTabbedMenu options, IModTabMenu menu)
		{
			if (!modConfigMenu.ModData.Config.Enabled)
			{
				return;
			}

			var rebindableIds = modConfigMenu.Mod.ModHelper.RebindingHelper.Rebindables;
			var rebindingElementTemplate = options.InputTab.Menu.transform.Find("MenuGeneral").Find("UIElement-Pause");

			foreach (var id in rebindableIds)
			{
				// sorry, but i have no idea how to extend the current menu system for this. so im just doing it the hacky way
				var newRebindingElement = UnityEngine.Object.Instantiate(rebindingElementTemplate);
				newRebindingElement.transform.parent = menu.Menu.transform.Find("Scroll View").Find("Viewport").Find("Content");
				newRebindingElement.transform.localScale = Vector3.one;
				newRebindingElement.GetComponent<KeyRebindingElement>().SetValue("_rebindId", id);
				newRebindingElement.GetComponent<KeyRebindingElement>().Initialize();
			}
		}

		private IModButton CreateButton(IModTabbedMenu options, string name)
		{
			var modButton = options.GameplayTab.Buttons.First().Copy(name);
			modButton.Button.enabled = true;
			if ((modButton.Button.FindSelectableOnRight() != null) || (modButton.Button.FindSelectableOnLeft() != null))
			{
				var nav = modButton.Button.navigation;
				nav.selectOnLeft = null;
				nav.selectOnRight = null;
				modButton.Button.navigation = nav;
			}
			modButton.Button.gameObject.name = name + "_button";
			GameObject.Destroy(modButton.Button.GetComponent<TabButton>());
			var menuOpt = modButton.Button.gameObject.AddComponent<MenuOption>();
			menuOpt.SetSelectable(modButton.Button);
			_menuOptions.Add(menuOpt);
			return modButton;
		}

		private static IModTabMenu CreateTab(IModTabbedMenu options, string name, bool enable)
		{
			var modsTab = options.GraphicsTab.Copy(name);
			modsTab.BaseButtons.ForEach(x => x.Hide());
			modsTab.Menu.GetComponentsInChildren<SliderElement>(true).ToList().ForEach(x => x.gameObject.SetActive(false));
			modsTab.Menu.GetComponentsInChildren<OptionsSelectorElement>(true).ToList().ForEach(x => x.gameObject.transform.localScale = Vector3.zero);
			modsTab.Menu.gameObject.name = name;
			modsTab.TabButton.gameObject.name = name + "_tab";
			options.AddTab(modsTab, enable);
			return modsTab;
		}
	}
}
