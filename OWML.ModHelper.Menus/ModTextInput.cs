using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModTextInput : ModInputField<string>, IModTextInput
    {
        private string _value;

        public ModTextInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu) : base(element, menu, inputMenu)
        {
        }

        protected override void Open()
        {
            InputMenu.OnConfirm += OnConfirm;
            InputMenu.OnCancel += OnCancel;
            InputMenu.Open(InputType.Text, Value);
        }

        private void OnConfirm(string text)
        {
            OnCancel();
            Value = text;
        }

        private void OnCancel()
        {
            InputMenu.OnConfirm -= OnConfirm;
            InputMenu.OnCancel -= OnCancel;
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
            var copy = GameObject.Instantiate(ToggleElement);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModTextInput(copy, Menu, InputMenu);
        }

        public IModTextInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
