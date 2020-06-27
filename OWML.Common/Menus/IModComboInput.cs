namespace OWML.Common.Menus
{
    public interface IModComboInput : IModInput<string>
    {
        IModComboInput Copy();
        IModComboInput Copy(string key);
    }
}
