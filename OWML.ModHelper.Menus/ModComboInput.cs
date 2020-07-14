using OWML.Common.Menus;
using OWML.ModHelper.Events;
using OWML.ModHelper.Input;
using UnityEngine;
using UnityEngine.UI;
using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class ModComboInput : ModPopupInput<string>, IModComboInput
    {
        public IModLayoutButton Button { get; }
        protected readonly IModInputCombinationMenu InputMenu;

        private string _combination;
        private readonly IModInputHandler _inputHandler;

        public override string Value
        {
            get => _combination;
            set
            {
                _combination = value;
                UpdateLayout(value);
            }
        }

        public ModComboInput(TwoButtonToggleElement element, IModMenu menu, IModInputCombinationMenu inputMenu, IModInputHandler inputHandler)
            : base(element, menu)
        {
            _inputHandler = inputHandler;
            InputMenu = inputMenu;

            Button = new ModLayoutButton(element.GetValue<Button>("_buttonTrue"), menu);
            Subscribe(Button);

            var myParent = (RectTransform)Button.Button.transform.parent;
            var rectTransform = (RectTransform)Button.Layout.LayoutGroup.transform;
            rectTransform.sizeDelta = new Vector2(myParent.sizeDelta.x * 2, myParent.sizeDelta.y);
        }

        private void UpdateLayout(string currentCombination)
        {
            Button.Layout.Clear();
            var individualCombos = currentCombination.Split('/');
            for (var i = 0; i < individualCombos.Length; i++)
            {
                var keyStrings = individualCombos[i].Split('+');
                for (var j = 0; j < keyStrings.Length; j++)
                {
                    Button.Layout.AddPicture(_inputHandler.Textures.KeyTexture(keyStrings[j]), ModInputLibrary.ScaleDown);
                    if (j < keyStrings.Length - 1)
                    {
                        Button.Layout.AddText("+");
                    }
                }
                if (i < individualCombos.Length - 1)
                {
                    Button.Layout.AddText("/");
                }
            }
            Button.Layout.UpdateState();
        }

        protected override void Open()
        {
            base.Open();
            InputMenu.Title = Menu is IModConfigMenu menu ? $"{menu.Title}.{Title}" : Title;
            InputMenu.FillMenu(_combination);
            InputMenu.OnConfirm += OnConfirm;
            InputMenu.OnCancel += OnCancel;
            InputMenu.Open();
        }

        private void OnConfirm(string text)
        {
            Value = text;
            OnCancel();
        }

        private void OnCancel()
        {
            InputMenu.OnConfirm -= OnConfirm;
            InputMenu.OnClose -= OnCancel;
        }

        public IModComboInput Copy()
        {
            var copy = Object.Instantiate(ToggleElement);
            Object.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
            return new ModComboInput(copy, Menu, InputMenu, _inputHandler);
        }

        public IModComboInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
