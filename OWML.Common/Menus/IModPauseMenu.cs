namespace OWML.Common.Menus
{
    public interface IModPauseMenu : IModPopupMenu
    {
        IModTabbedMenu OptionsMenu { get; }

        IModTitleButton ResumeButton { get; }
        IModTitleButton OptionsButton { get; }
        IModTitleButton QuitButton { get; }

        void Initialize(SettingsManager settingsManager);
    }
}
