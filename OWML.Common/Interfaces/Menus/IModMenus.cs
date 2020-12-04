namespace OWML.Common.Interfaces.Menus
{
    public interface IModMenus
    {
        IModMainMenu MainMenu { get; }
        IModPauseMenu PauseMenu { get; }
        IModsMenu ModsMenu { get; }
        IModInputCombinationMenu InputCombinationMenu { get; }
        IModPopupManager PopupManager { get; }
    }
}
