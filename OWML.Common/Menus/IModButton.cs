using System;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModButton
    {
        event Action OnClick;
        int Index { get; set; }
        Button Button { get; }
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