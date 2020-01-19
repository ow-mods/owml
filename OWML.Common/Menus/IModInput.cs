using System;

namespace OWML.Common.Menus
{
    public interface IModInput<T>
    {
        event Action<T> OnChange;
        T Value { get; set; }
    }
}