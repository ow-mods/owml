namespace OWML.Common.Menus
{
    public interface IModComboInput : IModInput<string>
    {
        IModLayoutButton Button { get; }
        IModComboInput Copy();
        IModComboInput Copy(string key);
    }
}
