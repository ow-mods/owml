namespace OWML.Common.Interfaces.Menus
{
    public interface IModOWMenu : IModMenu
    {
        IModTabbedMenu OptionsMenu { get; }
        IModButton OptionsButton { get; }
        IModButton QuitButton { get; }
    }
}
