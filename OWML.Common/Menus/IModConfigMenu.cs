namespace OWML.Common.Menus
{
    public interface IModConfigMenu : IModPopupMenu
    {
        IModData ModData { get; }
        IModBehaviour Mod { get; }
    }
}
