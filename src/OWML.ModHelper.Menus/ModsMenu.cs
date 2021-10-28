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
			_modConfigMenus.ForEach(x => x.RemoveAllListeners());
			OwmlMenu.RemoveAllListeners();

			_menus = menus;
			Menu = owMenu.Menu;

			var modsButton = owMenu.OptionsButton.Duplicate(ModsTitle);
			var options = owMenu.OptionsMenu;

			var modsMenu = CreateModsMenu(options);
			modsButton.OnClick += modsMenu.Open;
			_modConfigMenus.ForEach(x => x.OnClosed += modsMenu.Open);
			OwmlMenu.OnClosed += modsMenu.Open;
		}

		private IModPopupMenu CreateModsMenu(IModTabbedMenu options)
		{
			_menuOptions.Clear();
			var modsTab = CreateTab(options, ModsTitle);

			var owmlButton = CreateButton(options, Constants.OwmlTitle);
			modsTab.AddButton((IModButtonBase)owmlButton, 0);
			var owmlTab = CreateTab(options, Constants.OwmlTitle);
			owmlTab.HideButton();
			InitConfigMenu(OwmlMenu, options, owmlTab);
			owmlButton.OnClick += () => owmlTab.Open();

			var enabledMods = _modConfigMenus.Where(modConfigMenu => modConfigMenu.ModData.Config.Enabled).ToList();
			var index = CreateBlockOfButtons(options, modsTab, enabledMods, 1, "ENABLED MODS");
			var disabledMods = _modConfigMenus.Except(enabledMods).ToList();
			CreateBlockOfButtons(options, modsTab, disabledMods, index, "DISABLED MODS");

			modsTab.Menu.SetValue("_menuOptions", _menuOptions.ToArray());
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
			separator.Element.transform.localScale = options.GameplayTab.Buttons.First().Button.transform.localScale;
			foreach (var modConfigMenu in configMenus)
			{
				var modButton = CreateButton(options, modConfigMenu.Manifest.Name);
				var modTab = CreateTab(options, modConfigMenu.Manifest.Name);
				modTab.HideButton();
				InitConfigMenu(modConfigMenu, options, modTab);
				modButton.OnClick += () => modTab.Open();
				menu.AddButton((IModButtonBase)modButton, index++);
			}
			return index;
		}

		private void InitConfigMenu(IModConfigMenuBase modConfigMenu, IModTabbedMenu options, IModTabMenu modTabMenu)
		{
			var toggleTemplate = options.GraphicsTab.ToggleInputs[0];
			var sliderTemplate = options.GraphicsTab.SliderInputs.Find(sliderInput => sliderInput.HasValueText) ?? options.InputTab.SliderInputs[0];
			var selectorTemplate = options.GraphicsTab.SelectorInputs[0];
			var textInputTemplate = new ModTextInput(selectorTemplate.Copy().SelectorElement, modConfigMenu, _menus.PopupManager);
			textInputTemplate.Hide();
			var numberInputTemplate = new ModNumberInput(selectorTemplate.Copy().SelectorElement, modConfigMenu, _menus.PopupManager);
			numberInputTemplate.Hide();
			var seperatorTemplate = new ModSeparator(modConfigMenu);
			modConfigMenu.Initialize(modTabMenu.Menu, toggleTemplate, sliderTemplate, textInputTemplate, numberInputTemplate, selectorTemplate, seperatorTemplate);
			modConfigMenu.UpdateUIValues();
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
			GameObject.Destroy(modButton.Button.GetComponent<TabButton>());
			var menuOpt = modButton.Button.gameObject.AddComponent<MenuOption>();
			menuOpt.SetSelectable(modButton.Button);
			_menuOptions.Add(menuOpt);
			return modButton;
		}

		private static IModTabMenu CreateTab(IModTabbedMenu options, string name)
		{
			var modsTab = options.AudioTab.Copy(name);
			modsTab.BaseButtons.ForEach(x => x.Hide());
			modsTab.Menu.GetComponentsInChildren<SliderElement>(true).ToList().ForEach(x => x.gameObject.SetActive(false));
			modsTab.Menu.GetComponentsInChildren<OptionsSelectorElement>(true).ToList().ForEach(x => x.gameObject.transform.localScale = Vector3.zero);
			modsTab.Menu.GetValue<TooltipDisplay>("_tooltipDisplay").GetComponent<Text>().color = Color.clear;
			options.AddTab(modsTab);
			return modsTab;
		}
	}
}
