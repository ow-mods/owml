namespace OWML.Common.Interfaces.Menus
{
    public interface IModToggleInput : IModInput<bool>
    {
        TwoButtonToggleElement Toggle { get; }
        IModButton YesButton { get; }
        IModButton NoButton { get; }
        IModToggleInput Copy();
        IModToggleInput Copy(string key);
    }
}
