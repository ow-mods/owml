namespace OWML.Common.Menus
{
    public interface IModTitleButton : IModButton
    {
        string Title { get; set; }

        new IModTitleButton Copy();
        IModTitleButton Copy(string title);
        IModTitleButton Copy(string title, int index);

        IModTitleButton Duplicate(string title);
        IModTitleButton Duplicate(string title, int index);

        IModTitleButton Replace(string title);
        IModTitleButton Replace(string title, int index);
    }
}