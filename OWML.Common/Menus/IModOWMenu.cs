namespace OWML.Common.Menus
{
    public interface IModOWMenu : IModMenu
    {
        IModTabbedMenu OptionsMenu { get; }
        IModTitleButton OptionsButton { get; }
        IModTitleButton QuitButton { get; }
    }
}
