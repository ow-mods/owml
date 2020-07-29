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
        public IModConfigMenuBase OwmlMenu { get; }
        public IModInputCombinationMenu InputCombinationMenu { get; }
        public IModPopupManager PopupManager { get; }

        public ModMenus(IModConsole console, IModEvents events, IModInputHandler inputHandler,
            IModManifest owmlManifest, IOwmlConfig owmlConfig, IOwmlConfig owmlDefaultConfig)
        {
            MainMenu = new ModMainMenu(console);
            PauseMenu = new ModPauseMenu(console);
            ModsMenu = new ModsMenu(console, this, inputHandler, events);
            OwmlMenu = new OwmlConfigMenu(console, owmlManifest, owmlConfig, owmlDefaultConfig);
            PopupManager = new ModPopupManager(console, inputHandler, events);
            InputCombinationMenu = new ModInputCombinationMenu(console);

            const string supportedVersion = "1.0.7";
            if (!Application.version.StartsWith(supportedVersion))
            {
                console.WriteLine($"Warning - Only version {supportedVersion} is supported for modded menus.\n" +
                                  "Please update the game.", MessageType.Warning);
                return;
            }
            events.Subscribe<SettingsManager>(Common.Events.AfterStart);
            events.Subscribe<TitleScreenManager>(Common.Events.AfterStart);
            events.Event += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
        {
            if (behaviour is SettingsManager settingsManager &&
                ev == Common.Events.AfterStart &&
                settingsManager.name == "PauseMenuManagers")
            {
                PauseMenu.Initialize(settingsManager);
                ModsMenu.Initialize(PauseMenu);
            }
            else if (behaviour is TitleScreenManager titleScreenManager &&
                     ev == Common.Events.AfterStart)
            {
                MainMenu.Initialize(titleScreenManager);
                var inputMenu = titleScreenManager
                    .GetComponent<ProfileMenuManager>()
                    .GetValue<PopupInputMenu>("_createProfileConfirmPopup");
                PopupManager.Initialize(inputMenu.transform.parent.gameObject);
                ModsMenu.Initialize(MainMenu);
            }
        }

    }
}
