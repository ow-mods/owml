using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModMenus : IModMenus
    {
        public IModMainMenu MainMenu { get; }
        public IModPauseMenu PauseMenu { get; }
        public IModsMenu ModsMenu { get; }
        public IModInputMenu InputMenu { get; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModMenus(IModLogger logger, IModConsole console, IModEvents events)
        {
            _logger = logger;
            _console = console;

            MainMenu = new ModMainMenu(logger, console);
            PauseMenu = new ModPauseMenu(logger, console);
            ModsMenu = new ModsMenu(logger, console, this);
            InputMenu = new ModInputMenu(logger, console);

            events.Subscribe<SettingsManager>(Common.Events.AfterStart);
            events.Subscribe<TitleScreenManager>(Common.Events.AfterStart);
            events.OnEvent += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
        {
            if (behaviour.GetType() == typeof(SettingsManager) && ev == Common.Events.AfterStart && behaviour.name == "PauseMenuManagers")
            {
                var settingsManager = (SettingsManager)behaviour;
                PauseMenu.Initialize(settingsManager);
                ModsMenu.Initialize(PauseMenu);
            }
            else if (behaviour.GetType() == typeof(TitleScreenManager) && ev == Common.Events.AfterStart)
            {
                var titleScreenManager = (TitleScreenManager)behaviour;
                MainMenu.Initialize(titleScreenManager);
                var inputMenu = titleScreenManager.GetComponent<ProfileMenuManager>().GetValue<PopupInputMenu>("_createProfileConfirmPopup");
                InputMenu.Initialize(inputMenu);
                ModsMenu.Initialize(MainMenu);
            }
        }

    }
}
