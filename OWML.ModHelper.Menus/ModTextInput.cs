using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

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
            InputMenu.OnInput += OnInput;
            InputMenu.Open(InputField.ContentType.Standard, InputField.CharacterValidation.None, _value);
        }

        private void OnInput(string text)
        {
            InputMenu.OnInput -= OnInput;
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
