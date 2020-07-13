namespace OWML.Common.Menus
{
    public interface IModOWMenu : IModMenu
    {
        IModTabbedMenu OptionsMenu { get; }
        IModButton OptionsButton { get; }
        IModButton QuitButton { get; }
    }
}
