#pragma warning disable 1591

namespace OWML.Common.Menus
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
