namespace OWML.Common
{
	public delegate void OptionValueChangedEvent(int newIndex, string newSelection);

	public interface IOWMLOptionsSelectorElement : IOWMLMenuValueOption
	{
		public event OptionValueChangedEvent OnValueChanged;
	}
}
