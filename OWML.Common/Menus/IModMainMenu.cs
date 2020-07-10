namespace OWML.Common.Menus
{
    public interface IModMainMenu : IModOWMenu
    {
        IModTitleButton ResumeExpeditionButton { get; }
        IModTitleButton NewExpeditionButton { get; }
        IModTitleButton ViewCreditsButton { get; }
        IModTitleButton SwitchProfileButton { get; }

        void Initialize(TitleScreenManager titleScreenManager);
    }
}
