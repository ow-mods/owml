using System;
using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public abstract class ModInputElement<T> : IModInput<T>
    {
        public event Action<T> OnChange;
        public abstract T Value { get; set; }
        public MonoBehaviour Element { get; }

        protected ModInputElement(MonoBehaviour element)
        {
            Element = element;
        }

        protected void InvokeOnChange(T value)
        {
            OnChange?.Invoke(value);
        }

    }
}
