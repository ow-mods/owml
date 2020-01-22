using OWML.Common;
using OWML.Common.Menus;

namespace OWML.ModHelper.Menus
{
    public class ModConfigMenu : ModPopupMenu, IModConfigMenu
    {
        public IModData ModData { get; }
        public IModBehaviour Mod { get; }

        public ModConfigMenu(IModLogger logger, IModConsole console, IModData modData, IModBehaviour mod) : base(logger, console)
        {
            ModData = modData;
            Mod = mod;
        }
        
    }
}
