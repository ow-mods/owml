using OWML.Common.Menus;
using OWML.ModHelper.Events;
using OWML.ModHelper.Input;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModComboInput : ModInput<string>, IModComboInput
    {
        private const float ScaleDown = 0.75f;

        public IModLayoutButton Button { get; }
        //protected readonly IModInputMenu InputMenu;
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

        public ModComboInput(TwoButtonToggleElement element, IModMenu menu, IModInputCombinationMenu inputMenu) : base(element, menu)
        {
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
                    AddKeySign(keyStrings[j]);
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

        private void AddKeySign(string key)
        {
            Button.Layout.AddPicture(
                ModInputLibrary.KeyTexture(key)
                , ScaleDown);
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
