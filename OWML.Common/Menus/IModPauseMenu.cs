namespace OWML.Common.Menus
{
    public interface IModPauseMenu : IModPopupMenu, IModOWMenu
    {
        IModTitleButton ResumeButton { get; }

        void Initialize(SettingsManager settingsManager);
    }
}
