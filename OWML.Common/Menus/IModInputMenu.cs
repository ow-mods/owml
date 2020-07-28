using System;

namespace OWML.Common.Menus
{
    public interface IModInputMenu : IModMenu
    {
        event Action<string> OnConfirm;
        event Action OnCancel;
    }
}
