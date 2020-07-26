using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.Logging;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModsMenu : ModPopupMenu, IModsMenu
    {
        private const string ModsTitle = "MODS";

        private readonly IModMenus _menus;
        private readonly IModInputHandler _inputHandler;
        private readonly List<IModConfigMenu> _modConfigMenus = new List<IModConfigMenu>();

        public ModsMenu(IModMenus menus, IModInputHandler inputHandler)
        {
            _menus = menus;
            _inputHandler = inputHandler;
        }

        public void AddMod(IModData modData, IModBehaviour mod)
        {
            _modConfigMenus.Add(new ModConfigMenu(modData, mod));
        }

        public IModConfigMenu GetModMenu(IModBehaviour modBehaviour)
        {
            ModConsole.OwmlConsole.WriteLine("Registering " + modBehaviour.ModHelper.Manifest.UniqueName);
            var modConfigMenu = _modConfigMenus.FirstOrDefault(x => x.Mod == modBehaviour);
            if (modConfigMenu == null)
            {
                ModConsole.OwmlConsole.WriteLine($"Error - {modBehaviour.ModHelper.Manifest.UniqueName} isn't added.", MessageType.Error);
                return null;
            }
            return modConfigMenu;
        }

        public void Initialize(IModOWMenu owMenu)
        {
            var modsButton = owMenu.OptionsButton.Duplicate(ModsTitle);
            var options = owMenu.OptionsMenu;

            InitCombinationMenu(options);

            var modsMenu = CreateModsMenu(options);
            modsButton.OnClick += () => modsMenu.Open();
            Menu = owMenu.Menu;

            InitConfigMenu(_menus.OwmlMenu, options);
            var owmlButton = modsButton.Duplicate(Constants.OwmlTitle);
            owmlButton.OnClick += () => _menus.OwmlMenu.Open();
        }

        private void InitCombinationMenu(IModTabbedMenu options)
        {
            if (_menus.InputCombinationMenu.Menu != null)
            {
                return;
            }
            var toggleTemplate = options.InputTab.ToggleInputs[0].Copy().Toggle;
            var comboElementTemplate = new ModInputCombinationElement(toggleTemplate,
                _menus.InputCombinationMenu, _menus.InputCombinationElementMenu, _inputHandler);
            comboElementTemplate.Hide();
            var rebindMenuTemplate = options.RebindingMenu.Copy().Menu;
            _menus.InputCombinationMenu.Initialize(rebindMenuTemplate, comboElementTemplate);
        }

        private IModPopupMenu CreateModsMenu(IModTabbedMenu options)
        {
            var modsTab = options.GameplayTab.Copy(ModsTitle);
            modsTab.BaseButtons.ForEach(x => x.Hide());
            modsTab.Menu.GetComponentsInChildren<Selectable>(true).ToList().ForEach(x => x.gameObject.SetActive(false));
            modsTab.Menu.GetValue<TooltipDisplay>("_tooltipDisplay").GetComponent<Text>().color = Color.clear;
            options.AddTab(modsTab);

            var enabledMods = _modConfigMenus.Where(modConfigMenu => modConfigMenu.ModData.Config.Enabled).ToList();
            var index = CreateBlockOfButtons(options, modsTab, enabledMods, 0, "ENABLED MODS");
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
                menu.AddButton(modButton, index++);
            }
            return index;
        }

        private void InitConfigMenu(IModConfigMenuBase modConfigMenu, IModTabbedMenu options)
        {
            var toggleTemplate = options.InputTab.ToggleInputs[0];
            var sliderTemplate = options.GraphicsTab.SliderInputs.Find(sliderInput => sliderInput.HasValueText) ?? options.InputTab.SliderInputs[0];
            var selectorTemplate = options.GraphicsTab.SelectorInputs[0];
            var textInputTemplate = new ModTextInput(toggleTemplate.Copy().Toggle, modConfigMenu, _menus.InputMenu);
            textInputTemplate.Hide();
            var comboInputTemplate = new ModComboInput(toggleTemplate.Copy().Toggle, modConfigMenu, _menus.InputCombinationMenu, _inputHandler);
            comboInputTemplate.Hide();
            var numberInputTemplate = new ModNumberInput(toggleTemplate.Copy().Toggle, modConfigMenu, _menus.InputMenu);
            numberInputTemplate.Hide();
            var rebindMenuCopy = options.RebindingMenu.Copy().Menu;
            modConfigMenu.Initialize(rebindMenuCopy, toggleTemplate, sliderTemplate, textInputTemplate,
                numberInputTemplate, comboInputTemplate, selectorTemplate);
        }

    }
}
