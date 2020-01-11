using System;

namespace OWML.Common
{
    public interface IModPopupMenu : IModMenu
    {
        Action OnOpen { get; set; }
        Action OnClose { get; set; }
        Action OnInit { get; set; }

        bool IsOpen { get; }

        void Open();
        void Close();
        void Toggle();

        IModPopupMenu CreateCopy(string name);
    }
}
