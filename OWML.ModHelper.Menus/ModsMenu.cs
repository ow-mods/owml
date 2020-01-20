using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;

namespace OWML.ModHelper.Menus
{
    public class ModsMenu : ModPopupMenu, IModsMenu
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        private readonly List<IModConfigMenu> _modConfigMenus;

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
            menus.PauseMenu.OnInit += () =>
            {
                var modsMenu = CreateModsMenu(menus);
                var modsButton = menus.PauseMenu.ResumeButton.Duplicate("MODS");
                modsButton.OnClick += () =>
                {
                    modsMenu.Open();
                };
            };
            // todo main menu
        }

        private IModPopupMenu CreateModsMenu(IModMenus menus)
        {
            var buttonTemplate = menus.PauseMenu.ResumeButton;
            var modsMenu = menus.PauseMenu.Copy("MODS");
            foreach (var button in modsMenu.Buttons)
            {
                button.Hide();
            }
            var modMenuTemplate = menus.PauseMenu.Copy();
            foreach (var button in modMenuTemplate.Buttons)
            {
                button.Hide();
            }
            foreach (var modConfigMenu in _modConfigMenus)
            {
                var modButton = buttonTemplate.Copy(modConfigMenu.ModData.Manifest.Name);
                var modMenu = CreateModMenu(modConfigMenu, modMenuTemplate, buttonTemplate);
                modButton.OnClick += () => modMenu.Open();
                modsMenu.AddButton(modButton);
            }
            return modsMenu;
        }

        private IModConfigMenu CreateModMenu(IModConfigMenu modConfigMenu, IModPopupMenu menuTemplate, IModButton buttonTemplate)
        {
            var modMenu = menuTemplate.Copy(modConfigMenu.ModData.Manifest.Name);
            modConfigMenu.Initialize(modMenu.Menu);
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
