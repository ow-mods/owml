using OWML.Common;
using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModMenus : IModMenus
    {
        public IModMainMenu MainMenu { get; }
        public IModPauseMenu PauseMenu { get; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModMenus(IModLogger logger, IModConsole console, IModEvents events)
        {
            _logger = logger;
            _console = console;

            MainMenu = new ModMainMenu(logger, console);
            var titleScreenManager = GameObject.FindObjectOfType<TitleScreenManager>();
            MainMenu.Initialize(titleScreenManager);

            PauseMenu = new ModPauseMenu(logger, console);

            events.Subscribe<SettingsManager>(Common.Events.AfterStart);
            events.Subscribe<TitleScreenManager>(Common.Events.AfterAwake);
            events.OnEvent += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
        {
            if (behaviour.GetType() == typeof(SettingsManager) && ev == Common.Events.AfterStart && behaviour.name == "PauseMenuManagers")
            {
                var settingsManager = (SettingsManager)behaviour;
                PauseMenu.Initialize(settingsManager);
            }
            else if (behaviour.GetType() == typeof(TitleScreenManager) && ev == Common.Events.AfterAwake)
            {
                var titleScreenManager = (TitleScreenManager)behaviour;
                MainMenu.Initialize(titleScreenManager);
            }
        }

    }
}
