namespace OWML.Utils
{
	public static class MenuExtensions
	{
		public static void SetMessage(this PopupMenu menu, string message) => menu._labelText.text = message;
	}
}
