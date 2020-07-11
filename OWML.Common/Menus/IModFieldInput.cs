namespace OWML.Common.Menus
{
    public interface IModFieldInput<T> : IModInput<T>
    {
        IModTitleButton Button { get; }
    }
}
