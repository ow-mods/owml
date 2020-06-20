using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using OWML.Common.Menus;
using System.Collections.Generic;

namespace OWML.ModHelper.Menus
{
    public class LayoutManager : ILayoutManager
    {
        private const int FontSize = 36;
        private static readonly Vector2 NormalPivot = new Vector2(0.5f, 0.5f);

        public LayoutGroup LayoutGroup { get; private set; }
        public int ChildCount => LayoutGroup.transform.childCount;

        private readonly UIStyleManager _styleManager;
        private readonly UIStyleApplier _styleApplier;
        private readonly FieldInfo _texts;
        private readonly FieldInfo _foregrounds;
        private readonly Vector3 _scale;
        private readonly HashSet<Graphic> _constantGraphics = new HashSet<Graphic>();

        public LayoutManager(LayoutGroup layout, UIStyleManager styleManager, UIStyleApplier styleApplier, Vector3 scale, List<Graphic> constantGraphics) : this(layout, styleManager, styleApplier, scale)
        {
            constantGraphics.ForEach(x => _constantGraphics.Add(x));
        }

        public LayoutManager(LayoutGroup layout, UIStyleManager styleManager, UIStyleApplier styleApplier, Vector3 scale)
        {
            _scale = scale;
            _styleManager = styleManager;
            _styleApplier = styleApplier;
            LayoutGroup = layout;
            LayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            LayoutGroup.transform.localPosition = Vector3.zero;
            ((RectTransform)LayoutGroup.transform).pivot = new Vector2(0.5f, 0.5f);//center
            _texts = typeof(UIStyleApplier).GetField("_textItems", BindingFlags.NonPublic | BindingFlags.Instance);
            _foregrounds = typeof(UIStyleApplier).GetField("_foregroundGraphics", BindingFlags.NonPublic | BindingFlags.Instance);
            UpdateState();
        }

        public void UpdateState()
        {
            var currentTexts = LayoutGroup.gameObject.GetComponentsInChildren<Text>();
            _texts.SetValue(_styleApplier, currentTexts);
            Graphic[] currentGraphics = new Graphic[currentTexts.Length + _constantGraphics.Count];
            int i;
            for (i = 0; i < currentTexts.Length; i++)
            {
                currentGraphics[i] = currentTexts[i];
            }
            foreach (var graphic in _constantGraphics)
            {
                currentGraphics[i] = graphic;
                i++;
            }
            _foregrounds.SetValue(_styleApplier, currentGraphics);
        }

        public void Clear()
        {
            var childCount = LayoutGroup.transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                if (!_constantGraphics.Contains(LayoutGroup.transform.GetChild(i).gameObject.GetComponent<Graphic>()))
                {
                    GameObject.Destroy(LayoutGroup.transform.GetChild(i).gameObject);
                }
            }
        }

        public void AddText(string text)
        {
            AddTextAt(text, LayoutGroup.transform.childCount);
        }

        public void AddTextAt(string text, int index)
        {
            var textObject = new GameObject("Text", new Type[] { typeof(RectTransform) });
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
            textObject.transform.SetSiblingIndex(index);
        }
        public void AddPicture(Texture2D texture, float scale = 1.0f)
        {
            AddPictureAt(texture, LayoutGroup.transform.childCount, scale);
        }

        public void AddPictureAt(Texture2D texture, int index, float scale = 1.0f)
        {
            var keySprite = Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), NormalPivot);
            var keyObject = new GameObject("ButtonImage", new Type[] { typeof(RectTransform) });
            var keyPicture = keyObject.AddComponent<Image>();
            keyPicture.sprite = keySprite;
            keyPicture.SetLayoutDirty();
            keyObject.AddComponent<LayoutElement>();
            keyObject.transform.SetParent(LayoutGroup.transform);
            keyObject.transform.localScale = _scale;
            ((RectTransform)keyObject.transform).sizeDelta =
                new Vector2((float)texture.width * scale, (float)texture.height * scale);
            ((RectTransform)keyObject.transform).pivot = NormalPivot;
            keyObject.transform.SetSiblingIndex(index);
        }
    }
}
