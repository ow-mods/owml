namespace OWML.Common.Menus
{
    public interface IModPauseMenu : IModPopupMenu, IModOWMenu
    {
        IModTabbedMenu OptionsMenu { get; }
        IModButton OptionsButton { get; }
        IModButton QuitButton { get; }
        IModButton ResumeButton { get; }

        void Initialize(SettingsManager settingsManager);
    }
}
