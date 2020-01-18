using System;

namespace OWML.Common.Menus
{
    public interface IModPopupMenu : IModMenu
    {
        Action OnOpen { get; set; }
        Action OnClose { get; set; }

        bool IsOpen { get; }
        string Title { get; set; }

        void Open();
        void Close();
        void Toggle();

        IModPopupMenu Copy();
        IModPopupMenu Copy(string title);
        void Initialize(Menu menu);

        [Obsolete("Use Copy instead")]
        IModPopupMenu CreateCopy(string name);
    }
}
