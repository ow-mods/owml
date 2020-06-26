namespace OWML.Common.Menus
{
    public interface IModPauseMenu : IModPopupMenu
    {
        IModTabbedMenu OptionsMenu { get; }

        IModButton ResumeButton { get; }
        IModButton OptionsButton { get; }
        IModButton QuitButton { get; }

        void Initialize(SettingsManager settingsManager);
    }
}
