namespace OWML.Common.Interfaces
{
    public interface IBindingChangeListener
    {
        void Initialize(IModInputHandler inputHandler, IModEvents events);
    }
}