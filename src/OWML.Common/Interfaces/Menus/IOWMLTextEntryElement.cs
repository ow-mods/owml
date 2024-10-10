namespace OWML.Common
{
	public delegate void TextEntryConfirmEvent();

	public interface IOWMLTextEntryElement : IOWMLMenuValueOption
	{
		public event TextEntryConfirmEvent OnConfirmEntry;

		public string GetInputText();

		/// <summary>
		/// Sets the text that is displayed without updating the underlying option value.
		/// </summary>
		public void SetText(string text);

		/// <summary>
		/// Sets the underlying option value to the text, and updates the displayed text.
		/// </summary>
		public void SetValue(string text);
	}
}
