using OWML.Common;
using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModTextInput : ModFieldInput<string>, IModTextInput
    {
        private string _value;

        public ModTextInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu, IModEvents events)
            : base(element, menu, inputMenu, events) { }

        protected override void Open()
        {
            base.Open();
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
            var copy = Object.Instantiate(ToggleElement);
            Object.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
            return new ModTextInput(copy, Menu, InputMenu, Events);
        }

        public IModTextInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
