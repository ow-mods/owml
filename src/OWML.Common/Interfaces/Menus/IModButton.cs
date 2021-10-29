namespace OWML.Common.Menus
{
	public interface IModButton : IModButtonBase
	{
		string Title { get; set; }

		new IModButton Copy();

		IModButton Copy(string title);

		IModButton Copy(string title, int index);

		IModButton Duplicate(string title);

		IModButton Duplicate(string title, int index);

		IModButton Replace(string title);

		IModButton Replace(string title, int index);
	}
}