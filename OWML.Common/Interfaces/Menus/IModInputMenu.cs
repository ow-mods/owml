using System;

namespace OWML.Common.Interfaces.Menus
{
    public interface IModInputMenu : IModMenu
    {
        event Action<string> OnConfirm;
        event Action OnCancel;
    }
}
