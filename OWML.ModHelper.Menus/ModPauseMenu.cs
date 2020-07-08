using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;

namespace OWML.ModHelper.Menus
{
    public class ModPauseMenu : ModPopupMenu, IModPauseMenu
    {
        public IModTabbedMenu OptionsMenu { get; }

        public IModTitleButton ResumeButton { get; private set; }
        public IModTitleButton OptionsButton { get; private set; }
        public IModTitleButton QuitButton { get; private set; }

        public ModPauseMenu(IModConsole console) : base(console)
        {
            OptionsMenu = new ModOptionsMenu(console);
        }

        public void Initialize(SettingsManager settingsManager)
        {
            var pauseMenuManager = settingsManager.GetComponent<PauseMenuManager>();
            var optionsMenu = settingsManager.GetValue<TabbedMenu>("_mainSettingsMenu");
            OptionsMenu.Initialize(optionsMenu);

            var pauseMenu = pauseMenuManager.GetValue<Menu>("_pauseMenu");
            base.Initialize(pauseMenu);

            ResumeButton = Buttons.Single(x => x.Button.name == "Button-Unpause");
            OptionsButton = Buttons.Single(x => x.Button.name == "Button-Options");
            QuitButton = Buttons.Single(x => x.Button.name == "Button-ExitToMainMenu");

            InvokeOnInit();
        }

    }
}
