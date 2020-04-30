namespace OWML.Common.Menus
{
    public interface IModInputInput : IModInput<string>
    {
        IModInputInput Copy();
        IModInputInput Copy(string key);
    }
}
