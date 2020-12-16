namespace OWML.Common.Menus
{
	public interface IModNumberInput : IModFieldInput<float>
	{
		IModNumberInput Copy();

		IModNumberInput Copy(string key);
	}
}
