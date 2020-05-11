using System;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModPopupMenu : IModMenu
    {
        event Action OnOpen;
        event Action OnClose;

        bool IsOpen { get; }
        string Title { get; set; }

        void Open();
        void Close();
        void Toggle();

        IModPopupMenu Copy();
        IModPopupMenu Copy(string title);
        void Initialize(Menu menu);
        void Initialize(Menu menu, LayoutGroup layoutGroup);

        [Obsolete("Use Copy instead")]
        IModPopupMenu CreateCopy(string name);
    }
}
