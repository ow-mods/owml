using System;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModButton
    {
        event Action OnClick;
        string Title { get; set; }
        int Index { get; set; }
        Button Button { get; }
        IModButton Copy();
        IModButton Duplicate();
        IModButton Replace();
        void Show();
        void Hide();
        void Initialize(IModMenu menu);
    }
}