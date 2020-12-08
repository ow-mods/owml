namespace OWML.Common.Interfaces.Menus
{
    public interface IModNumberInput : IModFieldInput<float>
    {
        IModNumberInput Copy();
        IModNumberInput Copy(string key);
    }
}
