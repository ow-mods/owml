namespace OWML.Common.Interfaces.Menus
{
    public interface IModComboInput : IModInput<string>
    {
        IModLayoutButton Button { get; }
        IModComboInput Copy();
        IModComboInput Copy(string key);
    }
}
