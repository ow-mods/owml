using OWML.Common.Interfaces;
using OWML.Common.Interfaces.Menus;
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

        public ModMenus(IModEvents events, IModInputHandler inputHandler, IModConfigMenuBase owmlMenu)
        {
            MainMenu = new ModMainMenu();
            PauseMenu = new ModPauseMenu();
            ModsMenu = new ModsMenu(this, owmlMenu, inputHandler, events);
            PopupManager = new ModPopupManager(inputHandler, events);
            InputCombinationMenu = new ModInputCombinationMenu();

            events.Subscribe<SettingsManager>(Common.Enums.Events.AfterStart);
            events.Subscribe<TitleScreenManager>(Common.Enums.Events.AfterStart);
            events.Event += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Enums.Events ev)
        {
            if (behaviour is SettingsManager settingsManager &&
                ev == Common.Enums.Events.AfterStart &&
                settingsManager.name == "PauseMenuManagers")
            {
                PauseMenu.Initialize(settingsManager);
                ModsMenu.Initialize(PauseMenu);
            }
            else if (behaviour is TitleScreenManager titleScreenManager &&
                     ev == Common.Enums.Events.AfterStart)
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
