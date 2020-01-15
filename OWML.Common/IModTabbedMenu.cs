namespace OWML.Common
{
    public interface IModTabbedMenu : IModPopupMenu
    {
        IModPopupMenu GamePlay { get; }
        IModPopupMenu Audio { get; }
        IModPopupMenu Input { get; }
        IModPopupMenu Graphics { get; }

        new IModTabbedMenu Copy();
    }
}
