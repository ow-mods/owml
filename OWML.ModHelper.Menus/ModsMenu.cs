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

        private Transform _modMenuCanvasTemplate;
        private IModButton _modButtonTemplate;

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
            CreateModMenuTemplate(menus.MainMenu);
            menus.MainMenu.OnInit += () => InitMainMenu(menus.MainMenu);
            menus.PauseMenu.OnInit += () => InitPauseMenu(menus.PauseMenu);
        }

        private void CreateModMenuTemplate(IModMainMenu mainMenu)
        {
            var remapControlsButton = mainMenu.OptionsMenu.InputTab.GetButton("UIElement-RemapControls");
            var buttonTemplate = GameObject.Instantiate(remapControlsButton.Button);
            buttonTemplate.gameObject.AddComponent<DontDestroyOnLoad>();
            _modButtonTemplate = new ModButton(buttonTemplate, mainMenu);

            var submitActionMenu = remapControlsButton.Button.GetComponent<SubmitActionMenu>();
            var rebindingMenu = submitActionMenu.GetValue<Menu>("_menuToOpen");
            if (rebindingMenu == null)
            {
                _console.WriteLine("Error: rebindingMenu is null");
            }

            var rebindingCanvas = rebindingMenu.transform.parent;
            _console.WriteLine("parent of rebindingMenu: " + rebindingCanvas.name);

            _modMenuCanvasTemplate = GameObject.Instantiate(rebindingCanvas);
            _modMenuCanvasTemplate.gameObject.AddComponent<DontDestroyOnLoad>();
        }

        private void InitMainMenu(IModMainMenu mainMenu)
        {
            _console.WriteLine("MainMenu OnInit");
            var modsButton = mainMenu.OptionsButton.Duplicate("MODS");
            _console.WriteLine("Copied mods button");
            var optionsMenu = mainMenu.OptionsMenu;
            _console.WriteLine("got options menu");
            var modsMenu = CreateModsMenu(optionsMenu);
            modsButton.OnClick += () => modsMenu.Open();
            Menu = mainMenu.Menu;
        }

        private void InitPauseMenu(IModPauseMenu pauseMenu)
        {
            _console.WriteLine("PauseMenu OnInit");
            if (_modMenuCanvasTemplate == null)
            {
                _console.WriteLine("wtf _modMenuCanvasTemplate is null Owo");
            }
            var modsButton = pauseMenu.OptionsButton.Duplicate("MODS");
            _console.WriteLine("Copied mods button");
            var optionsMenu = pauseMenu.OptionsMenu;
            _console.WriteLine("got options menu");
            var modsMenu = CreateModsMenu(optionsMenu);
            modsButton.OnClick += () => modsMenu.Open();
            Menu = pauseMenu.Menu;
        }

        private IModPopupMenu CreateModsMenu(IModTabbedMenu options)
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
                var modButton = _modButtonTemplate.Copy(modConfigMenu.ModData.Manifest.Name);
                _console.WriteLine("Copied mod button: " + modButton.Title);
                var modMenu = CreateModMenu(modConfigMenu);
                modButton.OnClick += () => modMenu.Open();
                modsTab.AddButton(modButton);
            }
            _console.WriteLine("CreateModsMenu is done");
            return modsTab;
        }

        private IModConfigMenu CreateModMenu(IModConfigMenu modConfigMenu)
        {
            _console.WriteLine("CreateModMenu");
            if (_modMenuCanvasTemplate == null)
            {
                _console.WriteLine("Error: _modMenuCanvasTemplate is null");
                return null;
            }
            var modMenuTemplate = _modMenuCanvasTemplate.GetChild(0).GetComponent<Menu>();
            var modMenuCopy = GameObject.Instantiate(modMenuTemplate, _modMenuCanvasTemplate.transform);
            _console.WriteLine("instantiated modmenucopy");
            modConfigMenu.Initialize(modMenuCopy);
            _console.WriteLine("initialized modConfigMenu");
            modConfigMenu.Title = modConfigMenu.ModData.Manifest.Name;
            var index = 2;
            modConfigMenu.AddButton(_modButtonTemplate.Copy("Enabled"), index++); // todo
            modConfigMenu.AddButton(_modButtonTemplate.Copy("Requires VR"), index++); // todo
            foreach (var setting in modConfigMenu.ModData.Config.Settings)
            {
                modConfigMenu.AddButton(_modButtonTemplate.Copy(setting.Key), index++); // todo
            }
            modConfigMenu.AddButton(_modButtonTemplate.Copy("OK"), index); // todo
            return modConfigMenu;
        }

        private IModConfig ParseConfig(IModButton button)
        {
            _console.WriteLine("todo: implement ModsMenu.ParseConfig");
            return new ModConfig();
        }

    }
}
