namespace OWML.Common
{
	public delegate void FloatOptionValueChangedEvent(float newValue);

	public interface IOWMLSliderElement : IOWMLMenuValueOption
	{
		public event FloatOptionValueChangedEvent OnValueChanged;
	}
}
