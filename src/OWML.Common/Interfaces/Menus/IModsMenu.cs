namespace OWML.Common.Interfaces.Menus
{
    public interface IModsMenu : IModPopupMenu
    {
        void AddMod(IModData modData, IModBehaviour mod);

        IModConfigMenu GetModMenu(IModBehaviour modBehaviour);

        void Initialize(IModMenus menus, IModOWMenu mainMenu);
    }
}
