using System;
using OWML.Common.Menus;
using UnityEngine;

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

        protected ModInput(MonoBehaviour element)
        {
            Element = element;
        }

        public abstract IModInput<T> Copy();

        protected void InvokeOnChange(T value)
        {
            OnChange?.Invoke(value);
        }

    }
}
