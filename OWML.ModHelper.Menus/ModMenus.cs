using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModMenus : IModMenus
    {
        public IModMenu MainMenu { get; }
        public IModPopupMenu PauseMenu { get; }

        private readonly IModConsole _console;

        public ModMenus(IModLogger logger, IModConsole console)
        {
            _console = console;
            MainMenu = new ModMainMenu(logger, console);
            PauseMenu = new ModPauseMenu(logger, console);
        }

        public IModPopupMenu CreateMenu()
        {
            var original = PauseMenu ?? MainMenu;

            var copy = GameObject.Instantiate(original);

        }

    }

}
