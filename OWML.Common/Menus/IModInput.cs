using System;
using UnityEngine;

#pragma warning disable 1591

namespace OWML.Common.Menus
{
    public interface IModInput<T>
    {
        event Action<T> OnChange;
        T Value { get; set; }
        MonoBehaviour Element { get; }
        string Title { get; set; }
        int Index { get; set; }
        void Show();
        void Hide();
        void Initialize(IModMenu menu);
    }
}