namespace OWML.Common.Menus
{
    public interface IModPauseMenu : IModPopupMenu
    {
        IModButton ResumeButton { get; }
        IModButton OptionsButton { get; }
        IModButton QuitButton { get; }
    }
}
