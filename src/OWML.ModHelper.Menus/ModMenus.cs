﻿using OWML.Common.Interfaces;
using OWML.Common.Interfaces.Menus;
using OWML.Utils;
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

        public ModMenus(
            IModEvents events, 
            IModMainMenu mainMenu, 
            IModPauseMenu pauseMenu,
            IModsMenu modsMenu,
            IModPopupManager popupManager,
            IModInputCombinationMenu inputComboMenu)
        {
            MainMenu = mainMenu;
            PauseMenu = pauseMenu;
            ModsMenu = modsMenu;
            PopupManager = popupManager;
            InputCombinationMenu = inputComboMenu;

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
                ModsMenu.Initialize(this, PauseMenu);
            }
            else if (behaviour is TitleScreenManager titleScreenManager &&
                     ev == Common.Enums.Events.AfterStart)
            {
                MainMenu.Initialize(titleScreenManager);
                var inputMenu = titleScreenManager
                    .GetComponent<ProfileMenuManager>()
                    .GetValue<PopupInputMenu>("_createProfileConfirmPopup");
                PopupManager.Initialize(inputMenu.transform.parent.gameObject);
                ModsMenu.Initialize(this, MainMenu);
            }
        }

    }
}
