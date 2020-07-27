using OWML.Common.Menus;
using System;
using OWML.Common;
using Object = UnityEngine.Object;

namespace OWML.ModHelper.Menus
{
    public class ModNumberInput : ModFieldInput<float>, IModNumberInput
    {
        private float _value;

        public ModNumberInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu, IModEvents events)
            : base(element, menu, inputMenu, events) { }

        protected override void Open()
        {
            base.Open();
            InputMenu.OnConfirm += OnConfirm;
            InputMenu.OnCancel += OnCancel;
            InputMenu.Open(InputType.Number, Value.ToString());
        }

        private void OnConfirm(string text)
        {
            OnCancel();
            Value = Convert.ToSingle(text);
        }

        private void OnCancel()
        {
            InputMenu.OnConfirm -= OnConfirm;
            InputMenu.OnCancel -= OnCancel;
        }

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
            var copy = Object.Instantiate(ToggleElement);
            Object.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
            return new ModNumberInput(copy, Menu, InputMenu, Events);
        }

        public IModNumberInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
