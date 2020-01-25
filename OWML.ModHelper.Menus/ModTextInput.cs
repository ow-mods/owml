using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModTextInput : ModInput<string>, IModTextInput
    {
        public IModButton Button { get; }

        private readonly IModInputMenu _inputMenu;
        private readonly TwoButtonToggleElement _element;

        public ModTextInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu) : base(element, menu)
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
            _inputMenu.Open(Button.Title);
        }

        private void OnInput(string text)
        {
            Value = text;
        }

        public override string Value
        {
            get => Button.Title;
            set
            {
                Button.Title = value;
                InvokeOnChange(value);
            }
        }

        public IModTextInput Copy()
        {
            var copy = GameObject.Instantiate(_element);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModTextInput(copy, Menu, _inputMenu);
        }

        public IModTextInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
