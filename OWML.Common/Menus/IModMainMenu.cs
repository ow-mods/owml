namespace OWML.Common.Menus
{
    public interface IModMainMenu : IModOWMenu
    {
        IModTabbedMenu OptionsMenu { get; }
        IModButton OptionsButton { get; }
        IModButton QuitButton { get; }
        IModButton ResumeExpeditionButton { get; }
        IModButton NewExpeditionButton { get; }
        IModButton ViewCreditsButton { get; }
        IModButton SwitchProfileButton { get; }

        void Initialize(TitleScreenManager titleScreenManager);
    }
}
