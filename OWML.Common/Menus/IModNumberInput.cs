namespace OWML.Common.Menus
{
    public interface IModNumberInput : IModInputField<float>
    {
        IModNumberInput Copy();
        IModNumberInput Copy(string key);
    }
}
