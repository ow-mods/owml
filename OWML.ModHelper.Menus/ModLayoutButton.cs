using System.Reflection;
using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModLayoutButton : BaseButton, IModLayoutButton
    {
        private const int FontSize = 36;
        private static readonly Vector2 NormalPivot = new Vector2(0.5f, 0.5f);

        public HorizontalLayoutGroup LayoutGroup { get; }

        private readonly UIStyleManager _styleManager;
        private readonly UIStyleApplier _buttonStyleApplier;
        private readonly FieldInfo _texts;
        private readonly FieldInfo _foregrounds;
        private readonly Vector3 _scale;

        public ModLayoutButton(Button button, IModMenu menu) : base(button, menu)
        {
            _scale = button.transform.localScale;
            GameObject.Destroy(Button.GetComponentInChildren<Text>().gameObject);
            var layoutObject = new GameObject("LayoutGroup", typeof(RectTransform));
            layoutObject.transform.SetParent(button.transform);
            var target = layoutObject.AddComponent<Image>();
            target.raycastTarget = true;
            target.color = Color.clear;
            LayoutGroup = layoutObject.AddComponent<HorizontalLayoutGroup>();
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

        public override IBaseButton Copy()
        {
            var button = GameObject.Instantiate(Button);
            GameObject.Destroy(button.GetComponent<SubmitAction>());
            return new ModLayoutButton(button, Menu)
            {
                Index = Index + 1
            };
        }

        public override void AddToMenu(IBaseButton button)
        {
            Menu.AddLayoutButton((IModLayoutButton)button);
        }

        public void UpdateState()
        {
            var currentTexts = Button.gameObject.GetComponentsInChildren<Text>();
            _texts.SetValue(_buttonStyleApplier, currentTexts);
            _foregrounds.SetValue(_buttonStyleApplier, currentTexts);
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
