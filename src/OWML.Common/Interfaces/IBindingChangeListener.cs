namespace OWML.Common
{
	public interface IBindingChangeListener
	{
		void Initialize(IModInputHandler inputHandler, IModEvents events);
	}
}