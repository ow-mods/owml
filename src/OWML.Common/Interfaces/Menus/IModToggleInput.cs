namespace OWML.Common.Menus
{
	public interface IModToggleInput : IModInput<bool>
	{
		ToggleElement Toggle { get; }

		//IModButton YesButton { get; }

		//IModButton NoButton { get; }

		IModToggleInput Copy();

		IModToggleInput Copy(string key);
	}
}
