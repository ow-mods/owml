using System;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModLayoutButton
    {
        event Action OnClick;
        Button Button { get; }
        IModMenu Menu { get; }
        IModLayoutManager Layout { get; }
        int Index { get; set; }
        void Initialize(IModMenu menu);

        IModLayoutButton Copy();
        IModLayoutButton Copy(int index);

        IModLayoutButton Duplicate();
        IModLayoutButton Duplicate(int index);

        IModLayoutButton Replace();
        IModLayoutButton Replace(int index);

        void Show();
        void Hide();

        void SetControllerCommand(SingleAxisCommand inputCommand);
    }
}