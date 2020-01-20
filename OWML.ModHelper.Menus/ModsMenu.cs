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
            var originalButton = menus.PauseMenu.ResumeButton;
            var originalMenu = menus.PauseMenu.Copy();
            var modsMenu = menus.PauseMenu.Copy("MODS");
            foreach (var button in modsMenu.Buttons)
            {
                button.Hide();
            }
            foreach (var modConfigMenu in _modConfigMenus)
            {
                var modButton = CreateModButton(modConfigMenu, originalButton, originalMenu);
                modsMenu.AddButton(modButton);
            }
            return modsMenu;
        }

        private IModButton CreateModButton(IModConfigMenu modConfigMenu, IModButton originalModButton, IModPopupMenu originalMenu)
        {
            var modButton = originalModButton.Copy(modConfigMenu.ModData.Manifest.Name);
            var modMenu = originalMenu.Copy(modConfigMenu.ModData.Manifest.Name);
            modConfigMenu.Initialize(modMenu.Menu);
            modButton.OnClick += () =>
            {
                modMenu.Open();
            };
            return modButton;
        }

        private IModConfig ParseConfig(IModButton button)
        {
            _console.WriteLine("todo: implement ModsMenu.ParseConfig");
            return new ModConfig();
        }

    }
}
