using System;

namespace OWML.Common.Interfaces.Menus
{
    public interface IModInput<T> : IModInputBase
    {
        event Action<T> OnChange;
        T Value { get; set; }
    }
}