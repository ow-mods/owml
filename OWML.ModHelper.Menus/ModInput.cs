using System;
using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModInput<T> : IModInput<T>
    {
        public event Action<T> OnChange;
        public abstract T Value { get; set; }
        public MonoBehaviour Element { get; }

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

        private readonly Text _text;
        public string Title
        {
            get => _text.text;
            set => _text.text = value;
        }

        protected IModMenu Menu;

        protected ModInput(MonoBehaviour element, IModMenu menu)
        {
            Element = element;
            Menu = menu;
            _text = element.GetComponentInChildren<Text>();
        }

        protected void InvokeOnChange(T value)
        {
            OnChange?.Invoke(value);
        }

        public void Show()
        {
            Element.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Element.gameObject.SetActive(false);
        }

        public virtual void Initialize(IModMenu menu)
        {
            Menu = menu;
        }

    }
}
