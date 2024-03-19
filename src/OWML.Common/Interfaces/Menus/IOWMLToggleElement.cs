namespace OWML.Common
{
	public delegate void BoolOptionValueChangedEvent(bool newValue);

	public interface IOWMLToggleElement : IOWMLMenuValueOption
	{
		public event BoolOptionValueChangedEvent OnValueChanged;
		
	}
}
