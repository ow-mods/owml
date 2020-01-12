using System;

namespace OWML.Common
{
    public interface IModPopupMenu : IModMenu
    {
        Action OnOpen { get; set; }
        Action OnClose { get; set; }
        Action OnInit { get; set; }

        bool IsOpen { get; }
        string Title { get; set; }

        void Open();
        void Close();
        void Toggle();

        [Obsolete("Use Copy and Title instead")]
        IModPopupMenu CreateCopy(string name);
        IModPopupMenu Copy();
        void Initialize(Menu menu);
    }
}
