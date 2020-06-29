using OWML.Common.Menus;
using OWML.ModHelper.Events;
using OWML.ModHelper.Input;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace OWML.ModHelper.Menus
{
    public class ModComboInput : ModInput<string>, IModComboInput
    {
        public IModLayoutButton Button { get; }
        protected readonly IModInputMenu InputMenu;
        protected readonly TwoButtonToggleElement ToggleElement;

        private string _value;
        private readonly HorizontalLayoutGroup _layoutGroup;

        public ModComboInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu) : base(element, menu)
        {
            ToggleElement = element;
            InputMenu = inputMenu;
            Button = new ModLayoutButton(element.GetValue<Button>("_buttonTrue"), menu);
            Button.OnClick += Open;
            var noButton = ToggleElement.GetValue<Button>("_buttonFalse");
            noButton.transform.parent.gameObject.SetActive(false);
            _layoutGroup = Button.LayoutGroup;

            var parent = Button.Button.transform.parent;
            ((RectTransform)_layoutGroup.transform).sizeDelta = new Vector2(((RectTransform)parent).sizeDelta.x * 2, ((RectTransform)parent).sizeDelta.y);

            var layoutGroup = parent.parent.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;
            parent.GetComponent<LayoutElement>().preferredWidth = 100;
        }

        private void UpdateLayout(string currentCombination)
        {
            var childCount = _layoutGroup.transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(_layoutGroup.transform.GetChild(i).gameObject);
            }
            var individualCombos = currentCombination.Split('/');
            for (var i = 0; i < individualCombos.Length; i++)
            {
                var keyStrings = individualCombos[i].Split('+');
                for (var j = 0; j < keyStrings.Length; j++)
                {
                    Button.AddPicture(ModInputLibrary.KeyTexture(keyStrings[j]), ModInputLibrary.ScaleDown);
                    if (j < keyStrings.Length - 1)
                    {
                        Button.AddText("+");
                    }
                }
                if (i < individualCombos.Length - 1)
                {
                    Button.AddText("/");
                }
            }
            Button.UpdateState();
        }

        protected void Open()
        {
            InputMenu.OnConfirm += OnConfirm;
            InputMenu.OnCancel += OnCancel;
            InputMenu.Open(InputType.Text, _value);
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
                UpdateLayout(value);
            }
        }

        public IModComboInput Copy()
        {
            var copy = GameObject.Instantiate(ToggleElement);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModComboInput(copy, Menu, InputMenu);
        }

        public IModComboInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}