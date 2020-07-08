namespace OWML.Common.Menus
{
    public interface IModConfigMenu : IBaseConfigMenu
    {
        IModData ModData { get; }
        IModBehaviour Mod { get; }
    }
}
