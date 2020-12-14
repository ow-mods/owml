namespace OWML.Common.Menus
{
    public interface IModTextInput : IModFieldInput<string>
    {
        IModTextInput Copy();

        IModTextInput Copy(string key);
    }
}
