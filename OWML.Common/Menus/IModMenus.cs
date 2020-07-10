namespace OWML.Common.Menus
{
    public interface IModMenus
    {
        IModMainMenu MainMenu { get; }
        IModPauseMenu PauseMenu { get; }
        IModsMenu ModsMenu { get; }
        IBaseConfigMenu OwmlMenu { get; }
        IModInputMenu InputMenu { get; }
        IModInputCombinationElementMenu InputCombinationElementMenu { get; }
        IModInputCombinationMenu InputCombinationMenu { get; }
    }
}
