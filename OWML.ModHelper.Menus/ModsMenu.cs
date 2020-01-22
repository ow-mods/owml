using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModsMenu : ModPopupMenu, IModsMenu
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        private readonly List<IModConfigMenu> _modConfigMenus;

        private Menu _modMenuTemplate;

        public ModsMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            _modConfigMenus = new List<IModConfigMenu>();
        }

        public void AddMod(IModData modData, IModBehaviour mod)
        {
            _console.WriteLine("ModsMenu: AddMod");
            _modConfigMenus.Add(new ModConfigMenu(_logger, _console, modData, mod));
        }

        public IModConfigMenu Register(IModBehaviour modBehaviour)
        {
            _console.WriteLine("Registering " + modBehaviour.ModHelper.Manifest.UniqueName);
            var modConfigMenu = _modConfigMenus.FirstOrDefault(x => x.Mod == modBehaviour);
            if (modConfigMenu == null)
            {
                _console.WriteLine($"Error: {modBehaviour.ModHelper.Manifest.UniqueName} isn't added.");
                return null;
            }
            return modConfigMenu;
        }

        public void Initialize(IModMenus menus)
        {
            _console.WriteLine("ModsMenu: Initialize");
            menus.MainMenu.OnInit += () => InitMainMenu(menus.MainMenu);
            menus.PauseMenu.OnInit += () => InitPauseMenu(menus.PauseMenu);
        }

        private void InitMainMenu(IModMainMenu mainMenu)
        {
            _console.WriteLine("MainMenu OnInit");
            if (_modMenuTemplate == null)
            {
                CreateModMenuTemplate(mainMenu);
            }
            var modsButton = mainMenu.ResumeExpeditionButton.Duplicate("MODS");
            _console.WriteLine("Copied mods button");
            var optionsMenu = mainMenu.OptionsMenu;
            _console.WriteLine("got options menu");
            var modsMenu = CreateModsMenu(optionsMenu, mainMenu.ResumeExpeditionButton);
            modsButton.OnClick += () => modsMenu.Open();
            Menu = mainMenu.Menu;
        }

        private void CreateModMenuTemplate(IModMainMenu mainMenu)
        {
            var remapControlsButton = mainMenu.OptionsMenu.InputTab.Buttons.Single(x => x.Button.name == "UIElement-RemapControls");
            var submitActionMenu = remapControlsButton.Button.GetComponent<SubmitActionMenu>();
            var rebindingMenu = submitActionMenu.GetValue<Menu>("_menuToOpen");
            if (rebindingMenu == null)
            {
                _console.WriteLine("Error: rebindingMenu is null");
            }
            _modMenuTemplate = GameObject.Instantiate(rebindingMenu);
            _modMenuTemplate.gameObject.AddComponent<DontDestroyOnLoad>();
        }

        private void InitPauseMenu(IModPauseMenu pauseMenu)
        {
            _console.WriteLine("PauseMenu OnInit");
            var modsButton = pauseMenu.ResumeButton.Duplicate("MODS");
            _console.WriteLine("Copied mods button");
            var optionsMenu = pauseMenu.OptionsMenu;
            _console.WriteLine("got options menu");
            var modsMenu = CreateModsMenu(optionsMenu, pauseMenu.ResumeButton);
            modsButton.OnClick += () => modsMenu.Open();
            Menu = pauseMenu.Menu;
        }

        private IModPopupMenu CreateModsMenu(IModTabbedMenu options, IModButton buttonTemplate)
        {
            _console.WriteLine("CreateModsMenu");
            var modsTab = options.InputTab.Copy("MODS");
            _console.WriteLine("Copied tab");
            foreach (var button in modsTab.Buttons)
            {
                button.Hide();
            }
            options.AddTab(modsTab);
            _console.WriteLine("Added tab");
            foreach (var modConfigMenu in _modConfigMenus)
            {
                _console.WriteLine("Mod: " + modConfigMenu.ModData.Manifest.UniqueName);
                var modButton = buttonTemplate.Copy(modConfigMenu.ModData.Manifest.Name);
                _console.WriteLine("Copied mod button: " + modButton.Title);
                var modMenu = CreateModMenu(modConfigMenu, buttonTemplate);
                modButton.OnClick += () => modMenu.Open();
                modsTab.AddButton(modButton);
            }
            _console.WriteLine("CreateModsMenu is done");
            return modsTab;
        }

        private IModConfigMenu CreateModMenu(IModConfigMenu modConfigMenu, IModButton buttonTemplate)
        {
            _console.WriteLine("CreateModMenu");
            if (_modMenuTemplate == null)
            {
                _console.WriteLine("Error: _modMenuTemplate is null");
                return null;
            }
            var modMenuCopy = GameObject.Instantiate(_modMenuTemplate, buttonTemplate.Button.transform.root);
            _console.WriteLine("instantiated modmenucopy");
            modConfigMenu.Initialize(modMenuCopy);
            _console.WriteLine("initialized modConfigMenu");
            modConfigMenu.Title = modConfigMenu.ModData.Manifest.Name;
            var index = 2;
            modConfigMenu.AddButton(buttonTemplate.Copy("Enabled"), index++); // todo
            modConfigMenu.AddButton(buttonTemplate.Copy("Requires VR"), index++); // todo
            foreach (var setting in modConfigMenu.ModData.Config.Settings)
            {
                modConfigMenu.AddButton(buttonTemplate.Copy(setting.Key), index++); // todo
            }
            modConfigMenu.AddButton(buttonTemplate.Copy("OK"), index); // todo
            return modConfigMenu;
        }

        private IModConfig ParseConfig(IModButton button)
        {
            _console.WriteLine("todo: implement ModsMenu.ParseConfig");
            return new ModConfig();
        }

    }
}
