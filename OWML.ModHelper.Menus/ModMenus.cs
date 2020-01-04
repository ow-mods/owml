using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class ModMenus : IModMenus
    {
        public IModMenu MainMenu { get; }
        public IModMenu PauseMenu { get; }
        public IModMenu OptionsMenu { get; }

        public ModMenus(IModConsole console)
        {
            MainMenu = new ModMainMenu(console);
            PauseMenu = new ModPauseMenu(console);
            OptionsMenu = new ModOptionsMenu(console);
        }
    }

}
