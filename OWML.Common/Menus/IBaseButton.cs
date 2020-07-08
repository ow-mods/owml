using System;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IBaseButton
    {
        event Action OnClick;
        int Index { get; set; }
        Button Button { get; }
        void Initialize(IModMenu menu);

        IBaseButton Copy();
        IBaseButton Copy(int index);

        IBaseButton Duplicate();
        IBaseButton Duplicate(int index);

        IBaseButton Replace();
        IBaseButton Replace(int index);

        void Show();
        void Hide();

        void SetControllerCommand(SingleAxisCommand inputCommand);
    }
}