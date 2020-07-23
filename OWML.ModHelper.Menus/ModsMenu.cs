using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModsMenu : ModPopupMenu, IModsMenu
    {
        private const string ModsButtonTitle = "MODS";
        private const string OwmlButtonTitle = "OWML";

        private readonly IModMenus _menus;
        private readonly List<IModConfigMenu> _modConfigMenus;
        private readonly IModInputHandler _inputHandler;

        public ModsMenu(IModConsole console, IModMenus menus, IModInputHandler inputHandler) : base(console)
        {
            _menus = menus;
            _modConfigMenus = new List<IModConfigMenu>();
            _inputHandler = inputHandler;
        }

        public void AddMod(IModData modData, IModBehaviour mod)
        {
            _modConfigMenus.Add(new ModConfigMenu(OwmlConsole, modData, mod));
        }

        public IModConfigMenu GetModMenu(IModBehaviour modBehaviour)
        {
            OwmlConsole.WriteLine("Registering " + modBehaviour.ModHelper.Manifest.UniqueName);
            var modConfigMenu = _modConfigMenus.FirstOrDefault(x => x.Mod == modBehaviour);
            if (modConfigMenu == null)
            {
                OwmlConsole.WriteLine($"Error: {modBehaviour.ModHelper.Manifest.UniqueName} isn't added.");
                return null;
            }
            return modConfigMenu;
        }

        public void Initialize(IModOWMenu owMenu)
        {
            var modsButton = owMenu.OptionsButton.Duplicate(ModsButtonTitle);
            var options = owMenu.OptionsMenu;

            InitCombinationMenu(options);

            var modsMenu = CreateModsMenu(options);
            modsButton.OnClick += () => modsMenu.Open();
            Menu = owMenu.Menu;

            InitConfigMenu(_menus.OwmlMenu, options);
            var owmlButton = modsButton.Duplicate(OwmlButtonTitle);
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
            var modsTab = options.GameplayTab.Copy("MODS");
            modsTab.BaseButtons.ForEach(x => x.Hide());
            modsTab.Menu.GetComponentsInChildren<Selectable>(true).ToList().ForEach(x => x.gameObject.SetActive(false));
            modsTab.Menu.GetValue<TooltipDisplay>("_tooltipDisplay").GetComponent<Text>().color = Color.clear;
            options.AddTab(modsTab);
            var firstDisabled = true;
            var separator = new ModSeparator(modsTab) { Title = "ENABLED MODS" };
            modsTab.AddSeparator(separator, 0);
            separator.Element.transform.localScale = options.RebindingButton.Button.transform.localScale;
            int index = 1;
            foreach (var modConfigMenu in _modConfigMenus)
            {
                var modButton = options.RebindingButton.Copy(modConfigMenu.Manifest.Name);
                if (!modConfigMenu.ModData.Config.Enabled && firstDisabled)
                {
                    modsTab.AddSeparator(separator.Copy("DISABLED MODS"), index++);
                    firstDisabled = false;
                }
                modButton.Button.enabled = true;
                InitConfigMenu(modConfigMenu, options);
                modButton.OnClick += modConfigMenu.Open;
                modsTab.AddButton(modButton, index++);
            }
            modsTab.UpdateNavigation();
            modsTab.SelectFirst();
            return modsTab;
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
