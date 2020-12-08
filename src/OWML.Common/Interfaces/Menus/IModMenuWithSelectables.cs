using System;

namespace OWML.Common.Interfaces.Menus
{
    public interface IModMenuWithSelectables : IModPopupMenu
    {
        event Action OnCancel;
    }
}
