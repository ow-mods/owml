namespace OWML.ModHelper.Menus.CustomInputs
{
	public class OWMLMenuValueOption : MenuOption
	{
		public string ModSettingKey { get; set; }

		protected int _value;

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
	}
}
