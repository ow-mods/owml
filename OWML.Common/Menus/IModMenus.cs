namespace OWML.Common.Menus
{
    public interface IModMenus
    {
        IModMenu MainMenu { get; }
        IModPauseMenu PauseMenu { get; }
        IModTabbedMenu OptionsMenu { get; }
    }
}
