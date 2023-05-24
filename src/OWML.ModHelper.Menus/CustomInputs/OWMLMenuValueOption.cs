namespace OWML.ModHelper.Menus.CustomInputs
{
	public class OWMLMenuValueOption : MenuOption
	{
		protected int _value;

		public event OptionValueChangedEvent OnValueChanged;
		public delegate void OptionValueChangedEvent(int newValue);

		public virtual void Initialize(bool inputBool)
		{
			if (inputBool)
			{
				Initialize(1);
				return;
			}

			Initialize(0);
		}

		public virtual void Initialize(int startingValue)
		{
			base.Initialize();
			_value = startingValue;
		}

		public virtual bool GetValueAsBool() => _value > 0;

		public virtual int GetValue() => _value;

		public virtual void OnOptionValueChanged() => OnValueChanged?.Invoke(_value);
	}
}
