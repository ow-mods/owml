namespace OWML.Common.Interfaces.Menus
{
    public interface IModConfigMenu : IModConfigMenuBase
    {
        IModData ModData { get; }
        IModBehaviour Mod { get; }
    }
}
