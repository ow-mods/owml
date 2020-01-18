using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;

namespace OWML.ModHelper.Menus
{
    public class ModPauseMenu : ModPopupMenu, IModPauseMenu
    {
        public IModTabbedMenu OptionsMenu { get; }

        public IModButton ResumeButton { get; private set; }
        public IModButton OptionsButton { get; private set; }
        public IModButton QuitButton { get; private set; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModPauseMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            OptionsMenu = new ModOptionsMenu(logger, console);
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
