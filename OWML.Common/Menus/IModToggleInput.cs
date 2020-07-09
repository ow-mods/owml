namespace OWML.Common.Menus
{
    public interface IModToggleInput : IModInput<bool>
    {
        TwoButtonToggleElement Toggle { get; }
        IModTitleButton YesButton { get; }
        IModTitleButton NoButton { get; }
        IModToggleInput Copy();
        IModToggleInput Copy(string key);
    }
}
