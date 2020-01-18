using System.Collections.Generic;
using OWML.Common;
using OWML.Common.Menus;

namespace OWML.ModHelper.Menus
{
    public class ModsMenu : ModPopupMenu, IModsMenu
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private readonly List<IModBehaviour> _registeredMods;

        public ModsMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            _registeredMods = new List<IModBehaviour>();
        }

        public void Register(IModBehaviour modBehaviour)
        {
            _console.WriteLine("Registering " + modBehaviour.ModHelper.Manifest.UniqueName);
            if (_registeredMods.Contains(modBehaviour))
            {
                _console.WriteLine("Warning: " + modBehaviour.ModHelper.Manifest.UniqueName + " is already registered.");
                return;
            }
            _registeredMods.Add(modBehaviour);
        }

        public void Initialize(IModMenus menus)
        {
            menus.PauseMenu.OnInit += () =>
            {
                var modsMenu = CreateModsMenu(menus.PauseMenu);
                var modsButton = menus.PauseMenu.ResumeButton.Duplicate("MODS");
                modsButton.OnClick += () =>
                {
                    modsMenu.Open();
                };
            };
            // todo main menu
        }

        private IModPopupMenu CreateModsMenu(IModPauseMenu pauseMenu)
        {
            var originalButton = pauseMenu.ResumeButton;
            var modsMenu = pauseMenu.Copy("MODS");
            foreach (var button in modsMenu.Buttons)
            {
                button.Hide();
            }
            foreach (var mod in _registeredMods)
            {
                var modButton = CreateModButton(originalButton, mod);
                modsMenu.AddButton(modButton);
            }
            return modsMenu;
        }

        private IModButton CreateModButton(IModButton original, IModBehaviour mod)
        {
            var modButton = original.Copy(mod.ModHelper.Manifest.Name);
            return modButton;
            //button.OnClick += () => todo
            //{
            //    var config = ParseConfig(button);
            //    modBehaviour.ModHelper.Storage.Save(config, "config.json");
            //    modBehaviour.Configure(config);
            //};
        }

        private IModConfig ParseConfig(IModButton button)
        {
            _console.WriteLine("todo: implement ModsMenu.ParseConfig");
            return new ModConfig();
        }

    }
}
