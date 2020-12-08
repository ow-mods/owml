namespace OWML.Common.Interfaces.Menus
{
    public interface IModFieldInput<T> : IModInput<T>
    {
        IModButton Button { get; }
    }
}
