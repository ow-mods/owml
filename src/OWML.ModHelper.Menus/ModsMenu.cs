﻿using System.Collections.Generic;
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

		private readonly IModEvents _events;
		private readonly IModStorage _storage;
		private readonly List<IModConfigMenu> _modConfigMenus = new();
		private IModMenus _menus;

		public ModsMenu(
			IModConfigMenuBase owmlMenu,
			IModEvents events,
			IModStorage storage,
			IModConsole console)
				: base(console)
		{
			OwmlMenu = owmlMenu;
			_events = events;
			_storage = storage;
		}

		public void Init(IModMenus menus)
		{
			_menus = menus;
		}

		public void AddMod(IModData modData, IModBehaviour mod)
		{
			_modConfigMenus.Add(new ModConfigMenu(modData, mod, _storage, Console));
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
			_menus = menus;
			var modsButton = owMenu.OptionsButton.Duplicate(ModsTitle);
			var options = owMenu.OptionsMenu;

			InitCombinationMenu(options);

			var modsMenu = CreateModsMenu(options);
			modsButton.OnClick += () => modsMenu.Open();
			Menu = owMenu.Menu;
		}

		private void InitCombinationMenu(IModTabbedMenu options)
		{
			options.OnClosed += () => OnDeactivateOptions(options);
			
			var toggleTemplate = options.InputTab.ToggleInputs[0].Copy().Toggle;
			var rebindMenuTemplate = options.RebindingMenu.Copy().Menu;
		}

		private IModPopupMenu CreateModsMenu(IModTabbedMenu options)
		{
			var modsTab = options.GameplayTab.Copy(ModsTitle);
			modsTab.BaseButtons.ForEach(x => x.Hide());
			modsTab.Menu.GetComponentsInChildren<Selectable>(true).ToList().ForEach(x => x.gameObject.SetActive(false));
			modsTab.Menu.GetValue<TooltipDisplay>("_tooltipDisplay").GetComponent<Text>().color = Color.clear;
			options.AddTab(modsTab);

			var owmlButton = options.RebindingButton.Copy(Constants.OwmlTitle);
			modsTab.AddButton((IModButtonBase)owmlButton, 0);
			InitConfigMenu(OwmlMenu, options);
			owmlButton.OnClick += () => OwmlMenu.Open();

			var enabledMods = _modConfigMenus.Where(modConfigMenu => modConfigMenu.ModData.Config.Enabled).ToList();
			var index = CreateBlockOfButtons(options, modsTab, enabledMods, 1, "ENABLED MODS");
			var disabledMods = _modConfigMenus.Except(enabledMods).ToList();
			CreateBlockOfButtons(options, modsTab, disabledMods, index, "DISABLED MODS");

			modsTab.UpdateNavigation();
			modsTab.SelectFirst();
			return modsTab;
		}

		private int CreateBlockOfButtons(IModTabbedMenu options, IModTabMenu menu,
			List<IModConfigMenu> configMenus, int index, string title)
		{
			if (configMenus.Count <= 0)
			{
				return index;
			}
			var separator = new ModSeparator(menu)
			{
				Title = title
			};
			menu.AddSeparator(separator, index++);
			separator.Element.transform.localScale = options.RebindingButton.Button.transform.localScale;
			foreach (var modConfigMenu in configMenus)
			{
				var modButton = options.RebindingButton.Copy(modConfigMenu.Manifest.Name);
				modButton.Button.enabled = true;
				InitConfigMenu(modConfigMenu, options);
				modButton.OnClick += modConfigMenu.Open;
				menu.AddButton((IModButtonBase)modButton, index++);
			}
			return index;
		}

		private void InitConfigMenu(IModConfigMenuBase modConfigMenu, IModTabbedMenu options)
		{
			var toggleTemplate = options.InputTab.ToggleInputs[0];
			var sliderTemplate = options.GraphicsTab.SliderInputs.Find(sliderInput => sliderInput.HasValueText) ?? options.InputTab.SliderInputs[0];
			var selectorTemplate = options.GraphicsTab.SelectorInputs[0];
			var textInputTemplate = new ModTextInput(toggleTemplate.Copy().Toggle, modConfigMenu, _menus.PopupManager);
			textInputTemplate.Hide();
			var numberInputTemplate = new ModNumberInput(toggleTemplate.Copy().Toggle, modConfigMenu, _menus.PopupManager);
			numberInputTemplate.Hide();
			var rebindMenuCopy = options.RebindingMenu.Copy().Menu;
			modConfigMenu.Initialize(rebindMenuCopy, toggleTemplate, sliderTemplate, textInputTemplate,
				numberInputTemplate, selectorTemplate);
		}

		private void OnDeactivateOptions(IModTabbedMenu options)
		{
			if (!options.Menu.IsMenuEnabled() &&
				_modConfigMenus.Any(modMenu => modMenu.ModData.RequireReload))
			{
				_events.Unity.FireInNUpdates(ShowReloadWarning, 2);
			}
		}

		private void ShowReloadWarning()
		{
			var popup = _menus.PopupManager.CreateMessagePopup("Some changes in mod settings\nrequire a game reload\nto take effect", true, "Close game", "Reload later");
			popup.OnConfirm += OnPopupConfirm;
			popup.OnCancel += OnPopupCancel;
		}

		private void OnPopupCancel() => 
			_modConfigMenus.ForEach(modMenu => modMenu.ModData.UpdateSnapshot());

		private void OnPopupConfirm() => 
			Application.Quit();
	}
}
