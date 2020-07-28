using System;

namespace OWML.Common.Menus
{
    public interface IModMessagePopup : IModMenu
    {
        event Action OnConfirm;
        event Action OnCancel;
    }
}
