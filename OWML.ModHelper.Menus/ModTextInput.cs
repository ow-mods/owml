using OWML.Common.Menus;

namespace OWML.ModHelper.Menus
{
    public class ModTextInput : ModInput<string>, IModTextInput
    {
        private readonly IModButton _button;
        private IModInputMenu _inputMenu;

        public ModTextInput(IModButton button, IModInputMenu inputMenu, IModMenu menu) : base(button.Button, menu)
        {
            _button = button.Copy();
            _button.OnClick += () => Open(button.Title);
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
                _button.Title = value;
                InvokeOnChange(value);
            }
        }

        public IModTextInput Copy()
        {
            var copy = _button.Copy();
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
