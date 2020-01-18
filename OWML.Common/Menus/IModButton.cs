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
        void Initialize(IModMenu menu);

        IModButton Copy();
        IModButton Copy(string title);
        IModButton Copy(int index);
        IModButton Copy(string title, int index);

        IModButton Duplicate();
        IModButton Duplicate(string title);
        IModButton Duplicate(int index);
        IModButton Duplicate(string title, int index);

        IModButton Replace();
        IModButton Replace(string title);
        IModButton Replace(int index);
        IModButton Replace(string title, int index);

        void Show();
        void Hide();
    }
}