using System;
using UnityEngine;

namespace OWML.Common.Menus
{
    public interface IModInput<T>
    {
        event Action<T> OnChange;
        T Value { get; set; }
        MonoBehaviour Element { get; }
        IModInput<T> Copy();
    }
}