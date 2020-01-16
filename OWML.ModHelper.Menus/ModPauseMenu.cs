using System.Linq;
using OWML.Common;
using OWML.Common.Menus;

namespace OWML.ModHelper.Menus
{
    public class ModPauseMenu : ModPopupMenu, IModPauseMenu
    {
        public IModButton ResumeButton { get; private set; }
        public IModButton OptionsButton { get; private set; }
        public IModButton QuitButton { get; private set; }

        public ModPauseMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
        }

        public override void Initialize(Menu menu)
        {
            base.Initialize(menu);
            ResumeButton = Buttons.Single(x => x.Button.name == "Button-Unpause");
            OptionsButton = Buttons.Single(x => x.Button.name == "Button-Options");
            QuitButton = Buttons.Single(x => x.Button.name == "Button-ExitToMainMenu");
        }
    }
}
