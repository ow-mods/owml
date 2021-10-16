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
			var modsTab = CreateTab(options, ModsTitle);

			var owmlButton = options.GameplayTab.Buttons.First().Copy(Constants.OwmlTitle);
			modsTab.AddButton((IModButtonBase)owmlButton, 0);
			var owmlTab = CreateTab(options, Constants.OwmlTitle);
			owmlTab.HideButton();
			InitConfigMenu(OwmlMenu, options, owmlTab);
			owmlButton.OnClick += () => owmlTab.Open();

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
			separator.Element.transform.localScale = options.GameplayTab.Buttons.First().Button.transform.localScale;
			foreach (var modConfigMenu in configMenus)
			{
				var modButton = options.GameplayTab.Buttons.First().Copy(modConfigMenu.Manifest.Name);
				modButton.Button.enabled = true;
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
			modConfigMenu.Initialize(modTabMenu.Menu, toggleTemplate, sliderTemplate, textInputTemplate, numberInputTemplate, selectorTemplate);
			modConfigMenu.UpdateUIValues();
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
