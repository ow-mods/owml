using UnityEngine;
using UnityEngine.UI;
using OWML.Common.Menus;
using System.Collections.Generic;
using System.Linq;

namespace OWML.ModHelper.Menus
{
    public class ModLayoutManager : IModLayoutManager
    {
        private const int FontSize = 36;
        private static readonly Vector2 NormalPivot = new Vector2(0.5f, 0.5f);

        public LayoutGroup LayoutGroup { get; }
        public int ChildCount => LayoutGroup.transform.childCount;

        private readonly UIStyleManager _styleManager;
        private readonly ModUIStyleApplier _styleApplier;
        private readonly Vector3 _scale;
        private readonly HashSet<Graphic> _constantGraphics = new HashSet<Graphic>();
        private readonly HashSet<Graphic> _backingGraphics = new HashSet<Graphic>();

        public ModLayoutManager(LayoutGroup layout, UIStyleManager styleManager, ModUIStyleApplier styleApplier,
            Vector3 scale, Graphic[] constantGraphics, Graphic[] backGraphics)
            : this(layout, styleManager, styleApplier, scale, constantGraphics)
        {
            backGraphics.ToList().ForEach(element => _backingGraphics.Add(element));
            styleApplier.SetBackground(backGraphics);
        }

        public ModLayoutManager(LayoutGroup layout, UIStyleManager styleManager, ModUIStyleApplier styleApplier,
            Vector3 scale, Graphic[] constantGraphics)
            : this(layout, styleManager, styleApplier, scale)
        {
            constantGraphics.ToList().ForEach(element => _constantGraphics.Add(element));
        }

        public ModLayoutManager(LayoutGroup layout, UIStyleManager styleManager, ModUIStyleApplier styleApplier, Vector3 scale)
        {
            _scale = scale;
            _styleManager = styleManager;
            _styleApplier = styleApplier;
            LayoutGroup = layout;
            LayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            LayoutGroup.transform.localPosition = Vector3.zero;
            var rectTransform = (RectTransform)LayoutGroup.transform;
            rectTransform.pivot = NormalPivot;
            UpdateState();
        }

        public void UpdateState()
        {
            if (_styleApplier == null)
            {
                return;
            }
            var currentTexts = LayoutGroup.gameObject.GetComponentsInChildren<Text>();
            _styleApplier.SetTexts(currentTexts);
            var currentGraphics = currentTexts.Select(x => (Graphic)x).ToList();
            currentGraphics.AddRange(_constantGraphics);
            _styleApplier.SetForeground(currentGraphics.ToArray());
        }

        public void Clear()
        {
            if (LayoutGroup == null)
            {
                return;
            }
            foreach (Transform child in LayoutGroup.transform)
            {
                if (_constantGraphics.Contains(child.gameObject.GetComponent<Graphic>())
                    || _backingGraphics.Contains(child.gameObject.GetComponent<Graphic>()))
                {
                    continue;
                }
                child.gameObject.SetActive(false);
                Object.Destroy(child.gameObject);
            }
        }

        public void AddText(string text)
        {
            AddTextAt(text, LayoutGroup.transform.childCount);
        }

        public void AddTextAt(string text, int index)
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
            var rectTransform = (RectTransform)textObject.transform;
            rectTransform.sizeDelta = new Vector2(textComponent.preferredWidth, ((RectTransform)textObject.transform).sizeDelta.y);
            rectTransform.pivot = NormalPivot;
            textObject.transform.SetSiblingIndex(index);
        }

        public void AddPicture(Texture2D texture, float scale = 1.0f)
        {
            AddPictureAt(texture, LayoutGroup.transform.childCount, scale);
        }

        public void AddPictureAt(Texture2D texture, int index, float scale = 1.0f)
        {
            var keySprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), NormalPivot);
            var keyObject = new GameObject("ButtonImage", typeof(RectTransform));
            var keyPicture = keyObject.AddComponent<Image>();
            keyPicture.sprite = keySprite;
            keyPicture.SetLayoutDirty();
            keyObject.AddComponent<LayoutElement>();
            keyObject.transform.SetParent(LayoutGroup.transform);
            keyObject.transform.localScale = _scale;
            var rectTransform = (RectTransform)keyObject.transform;
            rectTransform.sizeDelta = new Vector2(texture.width * scale, texture.height * scale);
            rectTransform.pivot = NormalPivot;
            keyObject.transform.SetSiblingIndex(index);
        }
    }
}
