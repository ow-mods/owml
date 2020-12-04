using System;
using UnityEngine.UI;

namespace OWML.Common.Interfaces.Menus
{
    public interface IModButtonBase
    {
        event Action OnClick;
        int Index { get; set; }
        Button Button { get; }
        bool IsSelected { get; }
        void Initialize(IModMenu menu);

        IModButtonBase Copy();
        IModButtonBase Copy(int index);

        IModButtonBase Duplicate();
        IModButtonBase Duplicate(int index);

        IModButtonBase Replace();
        IModButtonBase Replace(int index);

        void Show();
        void Hide();
    }
}