using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace OWML.ModHelper.Menus
{
    public class ModComboInput : ModInput<string>, IModComboInput
    {
        private const float ScaleDown = 0.75f;
        private const string XboxPrefix = "xbox_";

        public IModLayoutButton Button { get; }
        protected readonly IModInputMenu InputMenu;
        protected readonly TwoButtonToggleElement ToggleElement;

        private string _value;
        private HorizontalLayoutGroup _layoutGroup;

        public ModComboInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu) : base(element, menu)
        {
            ToggleElement = element;
            InputMenu = inputMenu;
            Button = new ModLayoutButton(element.GetValue<Button>("_buttonTrue"), menu);
            Button.OnClick += Open;
            var noButton = ToggleElement.GetValue<Button>("_buttonFalse");
            noButton.transform.parent.gameObject.SetActive(false);
            _layoutGroup = Button.LayoutGroup;
            ((RectTransform)_layoutGroup.transform).sizeDelta = new Vector2(((RectTransform)Button.Button.transform.parent).sizeDelta.x * 2, ((RectTransform)Button.Button.transform.parent).sizeDelta.y);

            var layoutGroup = Button.Button.transform.parent.parent.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;
            Button.Button.transform.parent.GetComponent<LayoutElement>().preferredWidth = 100;
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
                    AddKeySign(keyStrings[j]);
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

        private Texture2D GetGamepadButtonTexture(string key)
        {
            return ButtonPromptLibrary.SharedInstance.GetButtonTexture((JoystickButton)Enum.Parse(typeof(JoystickButton), key));
        }

        private Texture2D GetXboxButtonTexture(string xboxKey)
        {
            switch (xboxKey[0])
            {
                case 'A':
                    return ButtonPromptLibrary.SharedInstance.GetButtonTexture(JoystickButton.FaceDown);
                case 'B':
                    return ButtonPromptLibrary.SharedInstance.GetButtonTexture(JoystickButton.FaceRight);
                case 'X':
                    return ButtonPromptLibrary.SharedInstance.GetButtonTexture(JoystickButton.FaceLeft);
                case 'Y':
                    return ButtonPromptLibrary.SharedInstance.GetButtonTexture(JoystickButton.FaceUp);
                default:
                    return GetGamepadButtonTexture(xboxKey);
            }
        }

        private void AddKeySign(string key)
        {
            var texture =
                key.Contains(XboxPrefix) ?
                GetXboxButtonTexture(key.Substring(XboxPrefix.Length)) :
                ButtonPromptLibrary.SharedInstance.GetButtonTexture((KeyCode)Enum.Parse(typeof(KeyCode), key));
            Button.AddPicture(texture, ScaleDown);
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
