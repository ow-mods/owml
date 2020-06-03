#pragma warning disable 1591

namespace OWML.Common.Menus
{
    public interface IModMenus
    {
        IModMainMenu MainMenu { get; }
        IModPauseMenu PauseMenu { get; }
        IModsMenu ModsMenu { get; }
        IModInputMenu InputMenu { get; }
    }
}
