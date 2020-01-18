namespace OWML.Common.Menus
{
    public interface IModsMenu
    {
        void Register(IModBehaviour modBehaviour);
        void Initialize(IModMenus menus);
    }
}
