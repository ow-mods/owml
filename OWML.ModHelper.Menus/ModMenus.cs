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
        public IBaseConfigMenu OwmlMenu { get; }
        public IModInputMenu InputMenu { get; }
        public IModInputCombinationElementMenu InputCombinationElementMenu { get; }
        public IModInputCombinationMenu InputCombinationMenu { get; }

        public ModMenus(IModConsole console, IModEvents events, IModInputHandler inputHandler,
            IModManifest owmlManifest, IOwmlConfig owmlConfig, IOwmlConfig owmlDefaultConfig)
        {
            MainMenu = new ModMainMenu(console);
            PauseMenu = new ModPauseMenu(console);
            ModsMenu = new ModsMenu(console, this, inputHandler);
            OwmlMenu = new OwmlConfigMenu(console, owmlManifest, owmlConfig, owmlDefaultConfig);
            InputMenu = new ModInputMenu(console);
            InputCombinationElementMenu = new ModInputCombinationElementMenu(console, inputHandler);
            InputCombinationMenu = new ModInputCombinationMenu(console);

            events.Subscribe<SettingsManager>(Common.Events.AfterStart);
            events.Subscribe<TitleScreenManager>(Common.Events.AfterStart);
            events.OnEvent += OnEvent;
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
