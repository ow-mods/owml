using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
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

        private IModToggleInput _toggleTemplate;
        private IModSliderInput _sliderTemplate;

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
            var modsButton = pauseMenu.OptionsButton.Duplicate("MODS");
            var optionsMenu = pauseMenu.OptionsMenu;
            var modsMenu = CreateModsMenu(optionsMenu);
            modsButton.OnClick += () => modsMenu.Open();
            Menu = pauseMenu.Menu;
        }

        private IModPopupMenu CreateModsMenu(IModTabbedMenu options)
        {
            _toggleTemplate = options.InputTab.GetToggleInput("UIElement-Rumble");
            _toggleTemplate.YesButton.Title = "Yes"; // todo doesn't work
            _toggleTemplate.NoButton.Title = "No";
            _sliderTemplate = options.InputTab.SliderInputs[0];
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
            modConfigMenu.GetButton("UIElement-CancelOutOfRebinding").Hide();

            var index = 2;
            AddConfigInput(modConfigMenu, "Enabled", modConfigMenu.ModData.Config.Enabled, index++);
            AddConfigInput(modConfigMenu, "Requires VR", modConfigMenu.ModData.Config.RequireVR, index++);
            foreach (var setting in modConfigMenu.ModData.Config.Settings)
            {
                AddConfigInput(modConfigMenu, setting, index++);
            }
            return modConfigMenu;
        }

        private void AddConfigInput(IModConfigMenu modConfigMenu, string name, object value, int index)
        {
            AddConfigInput(modConfigMenu, new KeyValuePair<string, object>(name, value), index);
        }

        private void AddConfigInput(IModConfigMenu modConfigMenu, KeyValuePair<string, object> setting, int index)
        {
            if (setting.Value is bool)
            {
                _console.WriteLine("for setting " + setting.Key + ", using type: toggle");
                var toggle = modConfigMenu.AddToggleInput(_toggleTemplate.Copy(setting.Key), index);
                // todo
                return;
            }

            if (new[] { typeof(long), typeof(int), typeof(float), typeof(double) }.Contains(setting.Value.GetType()))
            {
                // todo text input
                return;
            }

            if (setting.Value is JObject j)
            {
                var type = (string)j["type"];
                if (type == "slider")
                {
                    var min = (float)j["min"];
                    var max = (float)j["max"];
                    var value = (float)j["value"];
                    var slider = modConfigMenu.AddSliderInput(_sliderTemplate.Copy(setting.Key), index);
                    // todo
                    return;
                }
                if (type == "toggle")
                {
                    var left = (string)j["left"];
                    var right = (string)j["right"];
                    var value = (bool)j["value"];
                    var toggle = modConfigMenu.AddToggleInput(_toggleTemplate.Copy(setting.Key), index);
                    // todo
                    return;
                }

                _console.WriteLine("Error: unrecognized complex setting: " + setting.Value);
                return;
            }

            _console.WriteLine("Error: unrecognized setting type: " + setting.Value.GetType());
        }

        private IModConfig ParseConfig(IModButton button)
        {
            _console.WriteLine("todo: implement ModsMenu.ParseConfig");
            return new ModConfig();
        }

    }
}
