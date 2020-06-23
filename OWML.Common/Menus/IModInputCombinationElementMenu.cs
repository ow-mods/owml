using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common.Menus
{
    public interface IModInputCombinationElementMenu : IModMenu
    {
        event Action<string> OnConfirm;
        event Action OnCancel;

        void Initialize(PopupInputMenu menu);
        void Open(string value);
    }
}
