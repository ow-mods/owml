namespace OWML.Common.Menus
{
    public interface IModPauseMenu : IModPopupMenu, IModOWMenu
    {
        new IModTabbedMenu OptionsMenu { get; }

        new IModButton OptionsButton { get; }

        new IModButton QuitButton { get; }

        IModButton ResumeButton { get; }

        void Initialize(SettingsManager settingsManager);
    }
}
