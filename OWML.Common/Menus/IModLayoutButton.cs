using System;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModLayoutButton
    {
        event Action OnClick;
        HorizontalLayoutGroup LayoutGroup { get; }
        int Index { get; set; }
        Button Button { get; }
        void UpdateState();
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