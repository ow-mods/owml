namespace OWML.Common.Menus
{
    public interface IModTextInput : IModInputField<string>
    {
        IModTextInput Copy();
        IModTextInput Copy(string key);
    }
}
