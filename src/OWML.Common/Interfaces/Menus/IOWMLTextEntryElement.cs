namespace OWML.Common
{
	public delegate void TextEntryConfirmEvent();

	public interface IOWMLTextEntryElement : IOWMLMenuValueOption
	{
		public event TextEntryConfirmEvent OnConfirmEntry;

		public string GetInputText();
	}
}
