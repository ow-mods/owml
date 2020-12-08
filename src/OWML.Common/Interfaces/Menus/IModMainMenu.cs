namespace OWML.Common.Interfaces.Menus
{
    public interface IModMainMenu : IModOWMenu
    {
        new IModTabbedMenu OptionsMenu { get; }
        new IModButton OptionsButton { get; }
        new IModButton QuitButton { get; }
        IModButton ResumeExpeditionButton { get; }
        IModButton NewExpeditionButton { get; }
        IModButton ViewCreditsButton { get; }
        IModButton SwitchProfileButton { get; }

        void Initialize(TitleScreenManager titleScreenManager);
    }
}
