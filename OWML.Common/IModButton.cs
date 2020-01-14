using System;
using UnityEngine.UI;

namespace OWML.Common
{
    public interface IModButton
    {
        event Action OnClick;
        string Title { get; set; }
        int Index { get; set; }
        Button Button { get; }
        IModButton Copy();
        void Show();
        void Hide();
    }
}