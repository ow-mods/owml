using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModTextInput : ModFieldInput<string>, IModTextInput
    {
        private string _value;

        public ModTextInput(TwoButtonToggleElement element, IModMenu menu, IModPopupManager popupManager) : base(element, menu, popupManager)
        {
        }

        protected override void Open()
        {
            base.Open();
            var popup = PopupManager.CreateInputPopup(InputType.Text, Value.ToString());
            popup.OnConfirm += OnConfirm;
        }

        private void OnConfirm(string text)
        {
            Value = text;
        }

        public override string Value
        {
            get => _value;
            set
            {
                _value = value;
                Button.Title = value;
                InvokeOnChange(value);
            }
        }

        public IModTextInput Copy()
        {
            var copy = Object.Instantiate(ToggleElement);
            Object.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
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
