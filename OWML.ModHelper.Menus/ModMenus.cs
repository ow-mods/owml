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
        public IModInputCombinationMenu InputCombinationMenu { get; }
        public IModPopupManager PopupManager { get; }

        public ModMenus(IModConsole console, IModEvents events, IModInputHandler inputHandler, IModConfigMenuBase owmlMenu)
        {
            MainMenu = new ModMainMenu(console);
            PauseMenu = new ModPauseMenu(console);
            ModsMenu = new ModsMenu(console, this, owmlMenu, inputHandler, events);
            PopupManager = new ModPopupManager(console, inputHandler, events);
            InputCombinationMenu = new ModInputCombinationMenu(console);

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
