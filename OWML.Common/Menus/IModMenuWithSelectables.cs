using System;
using System.Collections.Generic;

namespace OWML.Common.Menus
{
    public interface IModMenuWithSelectables : IModPopupMenu
    {
        event Action OnCancel;
    }
}
