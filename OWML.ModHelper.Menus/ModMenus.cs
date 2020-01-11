using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class ModMenus : IModMenus
    {
        public IModMenu MainMenu { get; }
        public IModPopupMenu PauseMenu { get; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModMenus(IModLogger logger, IModConsole console, IModEvents events)
        {
            _logger = logger;
            _console = console;
            MainMenu = new ModMainMenu(logger, console);
            PauseMenu = new ModPauseMenu(logger, console, events);
        }

    }

}
