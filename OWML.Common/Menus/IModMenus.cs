namespace OWML.Common.Menus
{
    public interface IModMenus
    {
        IModMainMenu MainMenu { get; }
        IModPauseMenu PauseMenu { get; }
        IModsMenu ModsMenu { get; }
    }
}
