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
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        private readonly List<IModConfigMenu> _modConfigMenus;

        private Transform _modMenuTemplate;
        private IModButton _modButtonTemplate;

        public ModsMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            _modConfigMenus = new List<IModConfigMenu>();
        }

        public void AddMod(IModData modData, IModBehaviour mod)
        {
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
            _modMenuTemplate = GameObject.Instantiate(rebindingCanvas);
            _modMenuTemplate.gameObject.AddComponent<DontDestroyOnLoad>();
        }

        private void InitMainMenu(IModMainMenu mainMenu)
        {
            var modsButton = mainMenu.OptionsButton.Duplicate("MODS");
            var optionsMenu = mainMenu.OptionsMenu;
            var modsMenu = CreateModsMenu(optionsMenu);
            modsButton.OnClick += () => modsMenu.Open();
            Menu = mainMenu.Menu;
        }

        private void InitPauseMenu(IModPauseMenu pauseMenu)
        {
            if (_modMenuTemplate == null)
            {
                _console.WriteLine("wtf _modMenuCanvasTemplate is null Owo");
            }
            var modsButton = pauseMenu.OptionsButton.Duplicate("MODS");
            var optionsMenu = pauseMenu.OptionsMenu;
            var modsMenu = CreateModsMenu(optionsMenu);
            modsButton.OnClick += () => modsMenu.Open();
            Menu = pauseMenu.Menu;
        }

        private IModPopupMenu CreateModsMenu(IModTabbedMenu options)
        {
            var modsTab = options.InputTab.Copy("MODS");
            modsTab.Buttons.ForEach(x => x.Hide());
            modsTab.Menu.GetComponentsInChildren<Selectable>().ToList().ForEach(x => x.gameObject.SetActive(false));
            options.AddTab(modsTab);
            foreach (var modConfigMenu in _modConfigMenus)
            {
                var modButton = _modButtonTemplate.Copy(modConfigMenu.ModData.Manifest.Name);
                var modMenu = CreateModMenu(modConfigMenu);
                modButton.OnClick += () => modMenu.Open();
                modsTab.AddButton(modButton);
            }
            return modsTab;
        }

        private IModConfigMenu CreateModMenu(IModConfigMenu modConfigMenu)
        {
            if (_modMenuTemplate == null)
            {
                _console.WriteLine("Error: _modMenuCanvasTemplate is null");
                return null;
            }
            var modMenuTemplate = _modMenuTemplate.GetChild(0).GetComponent<Menu>();
            var modMenuCopy = GameObject.Instantiate(modMenuTemplate, _modMenuTemplate.transform);

            var layoutGroup = modMenuCopy.GetComponentsInChildren<VerticalLayoutGroup>().Single(x => x.name == "Content");
            modConfigMenu.Initialize(modMenuCopy, layoutGroup);
            modConfigMenu.Title = modConfigMenu.ModData.Manifest.Name;

            var index = 2;
            AddConfigButton(modConfigMenu, "Enabled", index++);
            AddConfigButton(modConfigMenu, "Requires VR", index++);
            foreach (var setting in modConfigMenu.ModData.Config.Settings)
            {
                AddConfigButton(modConfigMenu, setting.Key, index++);
            }
            AddConfigButton(modConfigMenu, "OK", index);
            return modConfigMenu;
        }

        private void AddConfigButton(IModConfigMenu modConfigMenu, string name, int index)
        {
            modConfigMenu.AddButton(_modButtonTemplate.Copy(name), index); // todo
        }

        private IModConfig ParseConfig(IModButton button)
        {
            _console.WriteLine("todo: implement ModsMenu.ParseConfig");
            return new ModConfig();
        }

    }
}
