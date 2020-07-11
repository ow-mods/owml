namespace OWML.Common.Menus
{
    public interface IModInputField<T> : IModInput<T>
    {
        IModTitleButton Button { get; }
    }
}
