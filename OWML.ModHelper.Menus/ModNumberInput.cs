using OWML.Common.Menus;
using OWML.ModHelper.Events;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModNumberInput : ModInput<float>, IModNumberInput
    {
        public IModButton Button { get; }

        private readonly IModInputMenu _inputMenu;
        private readonly TwoButtonToggleElement _element;

        private float _value;

        public ModNumberInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu) : base(element, menu)
        {
            _element = element;
            _inputMenu = inputMenu;
            Button = new ModButton(_element.GetValue<Button>("_buttonTrue"), menu);
            Button.OnClick += Open;

            var noButton = _element.GetValue<Button>("_buttonFalse");
            noButton.transform.parent.gameObject.SetActive(false);

            var layoutGroup = Button.Button.transform.parent.parent.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;

            Button.Button.transform.parent.GetComponent<LayoutElement>().preferredWidth = 100;
        }

        private void Open()
        {
            _inputMenu.OnInput += OnInput;
            _inputMenu.Open(InputField.CharacterValidation.Decimal, _value.ToString());
        }

        private void OnInput(string text)
        {
            _inputMenu.OnInput -= OnInput;
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
            var copy = GameObject.Instantiate(_element);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModNumberInput(copy, Menu, _inputMenu);
        }

        public IModNumberInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
