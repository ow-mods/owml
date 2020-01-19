using System;
using OWML.Common.Menus;

namespace OWML.ModHelper.Menus
{
    public abstract class ModInput<T> : IModInput<T>
    {
        public event Action<T> OnChange;
        public abstract T Value { get; set; }

        protected ModInput()
        {

        }

        protected void InvokeOnChange(T value)
        {
            OnChange?.Invoke(value);
        }

    }
}
