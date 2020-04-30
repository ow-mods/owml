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

        IModButton Copy();
        IModButton Copy(int index);

        IModButton Duplicate();
        IModButton Duplicate(int index);

        IModButton Replace();
        IModButton Replace(int index);

        void Show();
        void Hide();

        void SetControllerCommand(SingleAxisCommand inputCommand);
    }
}