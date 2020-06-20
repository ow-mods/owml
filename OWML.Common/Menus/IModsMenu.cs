namespace OWML.Common.Menus
{
    public interface IModsMenu : IModPopupMenu
    {
        void AddMod(IModData modData, IModBehaviour mod);
        IModConfigMenu GetModMenu(IModBehaviour modBehaviour);
        void Initialize(IModMainMenu mainMenu, PopupInputMenu inputMenu);
        void Initialize(IModPauseMenu pauseMenu);
    }
}
