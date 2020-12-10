using System;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModButton : IModButtonBase
    {
        event Action OnClick;

        int Index { get; set; }

        Button Button { get; }

        void Initialize(IModMenu menu);

        string Title { get; set; }

        new IModButton Copy();

        IModButton Copy(string title);

        IModButton Copy(string title, int index);

        IModButton Duplicate(string title);

        IModButton Duplicate(string title, int index);

        IModButton Replace(string title);

        IModButton Replace(string title, int index);

        void Show();

        void Hide();
    }
}