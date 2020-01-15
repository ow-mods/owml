using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModMenus : IModMenus
    {
        public IModMenu MainMenu { get; }
        public IModPopupMenu PauseMenu { get; }
        public IModTabbedMenu OptionsMenu { get; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModMenus(IModLogger logger, IModConsole console, IModEvents events)
        {
            _logger = logger;
            _console = console;

            MainMenu = new ModMainMenu(logger, console);
            PauseMenu = new ModPopupMenu(logger, console);
            OptionsMenu = new ModOptionsMenu(logger, console);

            events.Subscribe<SettingsManager>(Common.Events.AfterStart);
            events.OnEvent += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
        {
            if (behaviour.GetType() == typeof(SettingsManager) && 
                ev == Common.Events.AfterStart && 
                behaviour.name == "PauseMenuManagers")
            {
                var settingsManager = (SettingsManager)behaviour;
                InitPauseMenu(settingsManager);
                InitOptionsMenu(settingsManager);
            }
        }

        private void InitPauseMenu(SettingsManager settingsManager)
        {
            var pauseMenuManager = settingsManager.GetComponent<PauseMenuManager>();
            var pauseMenu = pauseMenuManager.GetValue<Menu>("_pauseMenu");
            PauseMenu.Initialize(pauseMenu);
        }

        private void InitOptionsMenu(SettingsManager settingsManager)
        {
            var optionsMenu = settingsManager.GetValue<TabbedMenu>("_mainSettingsMenu");
            OptionsMenu.Initialize(optionsMenu);
        }

    }
}
