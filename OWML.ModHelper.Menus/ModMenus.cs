using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class ModMenus : IModMenus
    {
        public IModMenu MainMenu { get; }
        public IModPopupMenu PauseMenu { get; }

        public ModMenus(IModLogger logger, IModConsole console)
        {
            MainMenu = new ModMainMenu(logger, console);
            PauseMenu = new ModPauseMenu(logger, console);
        }

        public IModPopupMenu CreateMenu()
        {
            throw new System.NotImplementedException();
        }

    }

}
