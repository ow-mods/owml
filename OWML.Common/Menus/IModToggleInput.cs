namespace OWML.Common.Menus
{
    public interface IModToggleInput : IModInput<bool>
    {
        IModButton YesButton { get; }
        IModButton NoButton { get; }
        IModToggleInput Copy();
        IModToggleInput Copy(string key);
    }
}
