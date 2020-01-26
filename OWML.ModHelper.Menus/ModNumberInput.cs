using OWML.Common.Menus;
using System;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModNumberInput : ModInputField<float>, IModNumberInput
    {
        private float _value;

        public ModNumberInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu) : base(element, menu, inputMenu)
        {
        }

        protected override void Open()
        {
            InputMenu.OnInput += OnInput;
            InputMenu.Open(InputType.Number, _value.ToString());
        }

        private void OnInput(string text)
        {
            InputMenu.OnInput -= OnInput;
            Value = Convert.ToSingle(text);
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
            var copy = GameObject.Instantiate(ToggleElement);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModNumberInput(copy, Menu, InputMenu);
        }

        public IModNumberInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
