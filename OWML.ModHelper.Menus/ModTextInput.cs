using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModTextInput : ModInput<string>, IModTextInput
    {
        private readonly IModToggleInput _toggleInput;
        private readonly IModButton _button;
        private readonly IModInputMenu _inputMenu;

        public ModTextInput(IModToggleInput toggleInput, IModInputMenu inputMenu, IModMenu menu) : base(toggleInput.Element, menu)
        {
            _toggleInput = toggleInput.Copy();
            GameObject.Destroy(_toggleInput.Element.GetComponent<TwoButtonToggleElement>());
            _toggleInput.NoButton.Button.transform.parent.gameObject.SetActive(false);
            _button = _toggleInput.YesButton;
            var layoutGroup = _button.Button.transform.parent.parent.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;
            _button.Button.transform.parent.GetComponent<LayoutElement>().preferredWidth = 100;
            _button.Title = "...";
            _button.OnClick += () => Open(_button.Title);
            _inputMenu = inputMenu;
        }

        private void Open(string title)
        {
            _inputMenu.OnInput += OnInput;
            _inputMenu.Open(title);
        }

        private void OnInput(string text)
        {
            Value = text;
        }

        public override string Value
        {
            get => _button.Title;
            set
            {
                ModConsole.Instance.WriteLine("in ModTextInput, setting title: " + value);
                _button.Title = value;
                InvokeOnChange(value);
            }
        }

        public IModTextInput Copy()
        {
            var copy = _toggleInput.Copy();
            return new ModTextInput(copy, _inputMenu, Menu);
        }

        public IModTextInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
