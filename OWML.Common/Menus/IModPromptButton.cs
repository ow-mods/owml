namespace OWML.Common.Menus
{
    public interface IModPromptButton : IModButton
    {
        string DefaultTitle { get; }
        ScreenPrompt Prompt { get; set; }

        new IModPromptButton Copy();
        IModPromptButton Copy(string title);
        IModPromptButton Copy(string title, int index);

        IModPromptButton Duplicate(string title);
        IModPromptButton Duplicate(string title, int index);

        IModPromptButton Replace(string title);
        IModPromptButton Replace(string title, int index);
    }
}