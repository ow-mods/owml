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
        public IModInputMenu InputMenu { get; }
        public IModInputCombinationElementMenu InputCombinationElementMenu { get; }
        public IModInputCombinationMenu InputCombinationMenu { get; }
        public IModMessagePopup MessagePopup { get; }

        public ModMenus(IModEvents events, IModInputHandler inputHandler,
            IModManifest owmlManifest, IOwmlConfig owmlConfig, IOwmlConfig owmlDefaultConfig)
        {
            MainMenu = new ModMainMenu();
            PauseMenu = new ModPauseMenu();
            ModsMenu = new ModsMenu(this, inputHandler, events);
            OwmlMenu = new OwmlConfigMenu(owmlManifest, owmlConfig, owmlDefaultConfig);
            InputMenu = new ModInputMenu();
            InputCombinationElementMenu = new ModInputCombinationElementMenu(inputHandler);
            MessagePopup = InputCombinationElementMenu.MessagePopup;
            InputCombinationMenu = new ModInputCombinationMenu();

            events.Subscribe<SettingsManager>(Common.Events.AfterStart);
            events.Subscribe<TitleScreenManager>(Common.Events.AfterStart);
            events.Event += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
        {
            if (behaviour.GetType() == typeof(SettingsManager) &&
                ev == Common.Events.AfterStart &&
                behaviour.name == "PauseMenuManagers")
            {
                var settingsManager = (SettingsManager)behaviour;
                PauseMenu.Initialize(settingsManager);
                ModsMenu.Initialize(PauseMenu);
            }
            else if (behaviour.GetType() == typeof(TitleScreenManager) &&
                     ev == Common.Events.AfterStart)
            {
                var titleScreenManager = (TitleScreenManager)behaviour;
                MainMenu.Initialize(titleScreenManager);
                var inputMenu = titleScreenManager
                    .GetComponent<ProfileMenuManager>()
                    .GetValue<PopupInputMenu>("_createProfileConfirmPopup");
                InputMenu.Initialize(inputMenu);
                InputCombinationElementMenu.Initialize(inputMenu);
                ModsMenu.Initialize(MainMenu);
            }
        }

    }
}
