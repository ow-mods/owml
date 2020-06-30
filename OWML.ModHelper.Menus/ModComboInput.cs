using OWML.Common.Menus;
using OWML.ModHelper.Events;
using OWML.ModHelper.Input;
using UnityEngine;
using UnityEngine.UI;
using OWML.Common;

namespace OWML.ModHelper.Menus
{
    public class ModComboInput : ModInput<string>, IModComboInput
    {
        public IModLayoutButton Button { get; }
        protected readonly IModInputCombinationMenu InputMenu;
        protected readonly TwoButtonToggleElement ToggleElement;

        private string _value;
        private HorizontalLayoutGroup _layoutGroup;
        public override string Value
        {
            get => _value;
            set
            {
                _value = value;
                UpdateLayout(value);
            }
        }

        public ModComboInput(TwoButtonToggleElement element, IModMenu menu, IModInputCombinationMenu inputMenu, IModInputHandler inputHandler) : base(element, menu)
        {
            _inputHandler = inputHandler;
            ToggleElement = element;
            InputMenu = inputMenu;
            Button = new ModLayoutButton(element.GetValue<Button>("_buttonTrue"), menu);
            Button.OnClick += Open;
            var noButton = ToggleElement.GetValue<Button>("_buttonFalse");
            noButton.transform.parent.gameObject.SetActive(false);
            _layoutGroup = (HorizontalLayoutGroup)Button.Layout.LayoutGroup;
            ((RectTransform)_layoutGroup.transform).sizeDelta = new Vector2(((RectTransform)Button.Button.transform.parent).sizeDelta.x * 2, ((RectTransform)Button.Button.transform.parent).sizeDelta.y);

            var parentLayoutGroup = Button.Button.transform.parent.parent.GetComponent<HorizontalLayoutGroup>();
            parentLayoutGroup.childControlWidth = true;
            parentLayoutGroup.childForceExpandWidth = true;
            Button.Button.transform.parent.GetComponent<LayoutElement>().preferredWidth = 100;
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
                    Button.Layout.AddPicture(ModInputLibrary.KeyTexture(key), ModInputLibrary.ScaleDown);
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

        protected void Open()
        {
            InputMenu.Combination = _value;
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
            var copy = GameObject.Instantiate(ToggleElement);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
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