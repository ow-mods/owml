using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common.Menus
{
    public interface IModInputCombinationMenu : IModPopupMenu
    {
        event Action<string> OnConfirm;
        string Combination { get; set; }
    }
}
