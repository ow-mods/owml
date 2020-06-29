using System;
using System.Reflection;
using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModLayoutButton : IModLayoutButton
    {
        private const int FontSize = 36;
        private static readonly Vector2 NormalPivot = new Vector2(0.5f, 0.5f);

        public event Action OnClick;
        public Button Button { get; }
        public IModMenu Menu { get; private set; }
        public HorizontalLayoutGroup LayoutGroup { get; }

        private int _index;
        private readonly UIStyleManager _styleManager;
        private readonly UIStyleApplier _buttonStyleApplier;
        private readonly FieldInfo _texts;
        private readonly FieldInfo _foregrounds;
        private readonly Vector3 _scale;

        public int Index
        {
            get => Button.transform.parent == null ? _index : Button.transform.GetSiblingIndex();
            set
            {
                _index = value;
                Button.transform.SetSiblingIndex(value);
            }
        }

        public ModLayoutButton(Button button, IModMenu menu)
        {
            _scale = button.transform.localScale;
            Button = button;
            Button.onClick.AddListener(() => OnClick?.Invoke());
            GameObject.Destroy(Button.GetComponentInChildren<Text>().gameObject);
            var layoutObject = new GameObject("LayoutGroup", typeof(RectTransform));
            layoutObject.transform.SetParent(button.transform);
            var target = layoutObject.AddComponent<Image>();
            target.raycastTarget = true;
            target.color = Color.clear;
            LayoutGroup = layoutObject.AddComponent<HorizontalLayoutGroup>();
            Initialize(menu);
            _buttonStyleApplier = Button.GetComponent<UIStyleApplier>();
            _styleManager = GameObject.FindObjectOfType<UIStyleManager>();
            LayoutGroup.childControlWidth = false;
            LayoutGroup.childControlHeight = false;
            LayoutGroup.childForceExpandHeight = false;
            LayoutGroup.childForceExpandWidth = false;
            LayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            LayoutGroup.transform.localPosition = Vector3.zero;
            ((RectTransform)LayoutGroup.transform).pivot = new Vector2(0.5f, 0.5f); //center
            _texts = typeof(UIStyleApplier).GetField("_textItems", BindingFlags.NonPublic | BindingFlags.Instance);
            _foregrounds = typeof(UIStyleApplier).GetField("_foregroundGraphics", BindingFlags.NonPublic | BindingFlags.Instance);
            UpdateState();
        }

        public void UpdateState()
        {
            var currentTexts = Button.gameObject.GetComponentsInChildren<Text>();
            _texts.SetValue(_buttonStyleApplier, currentTexts);
            _foregrounds.SetValue(_buttonStyleApplier, currentTexts);
        }

        public void Initialize(IModMenu menu)
        {
            Menu = menu;
        }

        public IModLayoutButton Copy()
        {
            var button = GameObject.Instantiate(Button);
            GameObject.Destroy(button.GetComponent<SubmitAction>());
            return new ModLayoutButton(button, Menu)
            {
                Index = Index + 1
            };
        }

        public IModLayoutButton Copy(int index)
        {
            var copy = Copy();
            copy.Index = index;
            return copy;
        }

        public IModLayoutButton Duplicate()
        {
            var copy = Copy();
            Menu.AddLayoutButton(copy);
            return copy;
        }

        public IModLayoutButton Duplicate(int index)
        {
            var dupe = Duplicate();
            dupe.Index = index;
            return dupe;
        }

        public IModLayoutButton Replace()
        {
            var duplicate = Duplicate();
            Hide();
            return duplicate;
        }

        public IModLayoutButton Replace(int index)
        {
            var replacement = Replace();
            replacement.Index = index;
            return replacement;
        }

        public void Show()
        {
            Button.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Button.gameObject.SetActive(false);
        }

        public void SetControllerCommand(SingleAxisCommand inputCommand)
        {
            Button.gameObject.AddComponent<ControllerButton>().Init(inputCommand);
        }

        public void AddText(string text)
        {
            var textObject = new GameObject("Text", typeof(RectTransform));
            var textComponent = textObject.AddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = FontSize;
            textComponent.font = _styleManager.GetMenuFont();
            textComponent.color = _styleManager.GetButtonForegroundMenuColor(UIElementState.NORMAL);
            textComponent.alignment = TextAnchor.MiddleCenter;
            textObject.AddComponent<LayoutElement>();
            textObject.transform.SetParent(LayoutGroup.transform);
            textObject.transform.localScale = _scale;
            ((RectTransform)textObject.transform).sizeDelta = new Vector2(textComponent.preferredWidth, ((RectTransform)textObject.transform).sizeDelta.y);
            ((RectTransform)textObject.transform).pivot = NormalPivot;
        }

        public void AddPicture(Texture2D texture, float scale = 1.0f)
        {
            var keySprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), NormalPivot);
            var keyObject = new GameObject("ButtonImage", typeof(RectTransform));
            var keyPicture = keyObject.AddComponent<Image>();
            keyPicture.sprite = keySprite;
            keyPicture.SetLayoutDirty();
            keyObject.AddComponent<LayoutElement>();
            keyObject.transform.SetParent(LayoutGroup.transform);
            keyObject.transform.localScale = _scale;
            ((RectTransform)keyObject.transform).sizeDelta = new Vector2(texture.width * scale, texture.height * scale);
            ((RectTransform)keyObject.transform).pivot = NormalPivot;
        }
    }
}
