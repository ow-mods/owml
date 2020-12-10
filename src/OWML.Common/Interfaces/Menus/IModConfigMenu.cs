namespace OWML.Common.Menus
{
    public interface IModConfigMenu : IModConfigMenuBase
    {
        IModData ModData { get; }
        IModBehaviour Mod { get; }
    }
}
