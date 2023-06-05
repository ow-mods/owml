using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
	public class ModTextInput : ModFieldInput<string>, IModTextInput
	{
		private string _value;

		public ModTextInput(OptionsSelectorElement element, IModMenu menu, IModPopupManager popupManager)
			: base(element, menu, popupManager)
		{
		}

		protected override void Open()
		{
			base.Open();
			var popup = PopupManager.CreateInputPopup(InputType.Text, Value);
			popup.OnConfirm += OnConfirm;
			popup.OnCancel += OnCancel;
		}

		private void OnConfirm(string text) => Value = text;

		private void OnCancel() => InvokeOnChange(Value);

		public override string Value
		{
			get => _value;
			set
			{
				_value = value;
				Button.Title = value;
				SelectorElement.Initialize(0, new string[] { value });
				InvokeOnChange(value);
			}
		}

		public IModTextInput Copy()
		{
			var copy = GameObject.Instantiate(SelectorElement);
			GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
			return new ModTextInput(copy, Menu, PopupManager);
		}

		public IModTextInput Copy(string title)
		{
			var copy = Copy();
			copy.Title = title;
			return copy;
		}
	}
}
