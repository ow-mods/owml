namespace OWML.Common.Menus
{
	public interface IModFieldInput<T> : IModInput<T>
	{
		IModButton Button { get; }
		
		void Initialize(string value);
	}
}
