using OWML.Common.Interfaces.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModSeparator : IModSeparator
    {
        private const int FontSize = 36;
        private const float MinimalHeight = 70f;

        public MonoBehaviour Element { get; }

        public LayoutElement LayoutElement { get; }

        private int _index;
        public int Index
        {
            get => Element.transform.parent == null ? _index : Element.transform.GetSiblingIndex();
            set
            {
                _index = value;
                Element.transform.SetSiblingIndex(value);
            }
        }
        public bool IsSelected => false;

        private readonly Text _text;
        public virtual string Title
        {
            get => _text.text;
            set => _text.text = value;
        }

        protected IModMenu Menu;

        public ModSeparator(LayoutElement layoutElement, IModMenu menu)
        {
            Menu = menu;
            LayoutElement = layoutElement;
            Element = layoutElement;
            _text = layoutElement.GetComponent<Text>();
        }

        public ModSeparator(IModMenu menu)
        {
            Menu = menu;
            var separatorObject = new GameObject("Separator");
            LayoutElement = separatorObject.AddComponent<LayoutElement>();
            Element = LayoutElement;
            var styleManager = GameObject.FindObjectOfType<UIStyleManager>();
            _text = separatorObject.AddComponent<Text>();
            _text.font = styleManager.GetMenuFont();
            _text.color = styleManager.GetForegroundMenuColor(UIElementState.NORMAL);
            _text.fontSize = FontSize;
            _text.alignment = TextAnchor.LowerCenter;
            LayoutElement.minHeight = MinimalHeight;
        }

        public virtual void Initialize(IModMenu menu)
        {
            Menu = menu;
        }

        public void Show()
        {
            Element.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Element.gameObject.SetActive(false);
        }

        public IModSeparator Copy()
        {
            var copy = GameObject.Instantiate(LayoutElement);
            return new ModSeparator(copy, Menu);
        }

        public IModSeparator Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }
    }
}
