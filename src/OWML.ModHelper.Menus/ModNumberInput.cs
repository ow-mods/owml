using System;
using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
	public class ModNumberInput : ModFieldInput<float>, IModNumberInput
	{
		private float _value;

		public ModNumberInput(OptionsSelectorElement element, IModMenu menu, IModPopupManager popupManager)
			: base(element, menu, popupManager)
		{
		}

		protected override void Open()
		{
			base.Open();
			var popup = PopupManager.CreateInputPopup(InputType.Number, Value.ToString());
			popup.OnConfirm += OnConfirm;
			popup.OnCancel += OnCancel;
		}

		private void OnConfirm(string text) => Value = Convert.ToSingle(text);

		private void OnCancel() => InvokeOnChange(Value);

		public override float Value
		{
			get => _value;
			set
			{
				_value = value;
				Button.Title = value.ToString();
				InvokeOnChange(value);
			}
		}

		public IModNumberInput Copy()
		{
			var copy = GameObject.Instantiate(SelectorElement);
			GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
			return new ModNumberInput(copy, Menu, PopupManager);
		}

		public IModNumberInput Copy(string title)
		{
			var copy = Copy();
			copy.Title = title;
			return copy;
		}
	}
}
