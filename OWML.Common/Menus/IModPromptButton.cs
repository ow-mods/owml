namespace OWML.Common.Menus
{
    public interface IModPromptButton : IModTitleButton
    {
        string DefaultTitle { get; }
        ScreenPrompt Prompt { get; set; }
    }
}