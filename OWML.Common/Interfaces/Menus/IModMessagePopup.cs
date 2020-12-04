using System;

namespace OWML.Common.Interfaces.Menus
{
    public interface IModMessagePopup : IModMenu
    {
        event Action OnConfirm;
        event Action OnCancel;
    }
}
