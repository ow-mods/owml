namespace OWML.Common.Menus
{
	public interface IModToggleInput : IModInput<bool>
	{
		ToggleElement Toggle { get; }

		public IModButton Button { get; }

		IModToggleInput Copy();

		IModToggleInput Copy(string key);
	}
}
