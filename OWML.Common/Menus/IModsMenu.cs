namespace OWML.Common.Menus
{
    public interface IModsMenu : IModPopupMenu
    {
        void AddMod(IModData modData, IModBehaviour mod);
        IModConfigMenu Register(IModBehaviour modBehaviour);
        void Initialize(IModMenus menus);
    }
}
