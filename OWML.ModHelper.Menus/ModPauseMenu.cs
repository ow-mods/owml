using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class ModPauseMenu : ModPopupMenu
    {
        private IModButton Resume;
        private IModButton Options;
        private IModButton Quit;
        
        public ModPauseMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
        }
    }
}
