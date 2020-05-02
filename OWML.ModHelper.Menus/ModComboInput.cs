using OWML.Common.Menus;
using OWML.ModHelper.Events;
using OWML.Common;
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
        private Vector3 _scale;
        private UIStyleManager _styleManager;
        private HorizontalLayoutGroup _layoutGroup;
        private readonly static int fontSize = 36;
        private readonly static Vector2 normalPivot = new Vector2(0.5f, 0.5f);
        private readonly static float scaleDown = 0.75f;

        public ModComboInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu) : base(element, menu)
        {
            ToggleElement = element;
            InputMenu = inputMenu;
            _scale = element.GetValue<Button>("_buttonTrue").transform.localScale;
            Button = new ModLayoutButton(element.GetValue<Button>("_buttonTrue"), menu);
            Button.OnClick += Open;
            var noButton = ToggleElement.GetValue<Button>("_buttonFalse");
            noButton.transform.parent.gameObject.SetActive(false);
            _layoutGroup = Button.LayoutGroup;
            ((RectTransform)_layoutGroup.transform).sizeDelta = new Vector2(((RectTransform)Button.Button.transform.parent).sizeDelta.x * 2, ((RectTransform)Button.Button.transform.parent).sizeDelta.y);
            _styleManager = MonoBehaviour.FindObjectOfType<UIStyleManager>();

            var layoutGroup = Button.Button.transform.parent.parent.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;
            Button.Button.transform.parent.GetComponent<LayoutElement>().preferredWidth = 100;
        }

        private void UpdateLayout(string currentCombination)
        {
            int childCount = _layoutGroup.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(_layoutGroup.transform.GetChild(i).gameObject);
            }
            string[] individualCombos = currentCombination.Split('/');
            for (int i = 0; i < individualCombos.Length; i++)
            {
                string[] keyStrings = individualCombos[i].Split('+');
                for (int j = 0; j < keyStrings.Length; j++)
                {
                    Texture2D keyTexture;
                    if (keyStrings[j].Contains("Xbox_"))
                    {
                        keyTexture = InputTranslator.GetButtonTexture((XboxButton)Enum.Parse(typeof(XboxButton), keyStrings[j].Substring(5)));
                    }
                    else
                    {
                        keyTexture = InputTranslator.GetButtonTexture((KeyCode)Enum.Parse(typeof(KeyCode), keyStrings[j]));
                    }
                    var keySprite = Sprite.Create(keyTexture, new Rect(0f, 0f, (float)keyTexture.width, (float)keyTexture.height), normalPivot);
                    var keyObject = new GameObject("ButtonImage", new Type[] { typeof(RectTransform) });
                    var keyPicture = keyObject.AddComponent<Image>();
                    keyPicture.sprite = keySprite;
                    keyPicture.SetLayoutDirty();
                    keyObject.AddComponent<LayoutElement>();
                    keyObject.transform.SetParent(_layoutGroup.transform);
                    keyObject.transform.localScale = _scale;
                    ((RectTransform)keyObject.transform).sizeDelta = new Vector2((float)keyTexture.width * scaleDown, (float)keyTexture.height * scaleDown);
                    ((RectTransform)keyObject.transform).pivot = normalPivot;
                    if (j < keyStrings.Length - 1)
                    {
                        AddText("+");
                    }
                }
                if (i < individualCombos.Length - 1)
                {
                    AddText("/");
                }
            }
            Button.UpdateState();
        }

        private void AddText(string txt)
        {
            var textObject = new GameObject("Text", new Type[] { typeof(RectTransform) });
            var text = textObject.AddComponent<Text>();
            text.text = txt;
            text.fontSize = fontSize;
            text.font = _styleManager.GetMenuFont();
            text.color = _styleManager.GetButtonForegroundMenuColor(UIElementState.NORMAL);
            text.alignment = TextAnchor.MiddleCenter;
            textObject.AddComponent<LayoutElement>();
            textObject.transform.SetParent(_layoutGroup.transform);
            textObject.transform.localScale = _scale;
            ((RectTransform)textObject.transform).sizeDelta = new Vector2(text.preferredWidth, ((RectTransform)textObject.transform).sizeDelta.y * scaleDown);
            ((RectTransform)textObject.transform).pivot = normalPivot;
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
