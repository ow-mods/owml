using System;
using UnityEngine;

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
    }
}