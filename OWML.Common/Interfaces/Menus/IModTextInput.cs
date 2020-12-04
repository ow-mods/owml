namespace OWML.Common.Interfaces.Menus
{
    public interface IModTextInput : IModFieldInput<string>
    {
        IModTextInput Copy();
        IModTextInput Copy(string key);
    }
}
