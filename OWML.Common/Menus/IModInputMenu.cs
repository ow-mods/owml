using System;

#pragma warning disable 1591

namespace OWML.Common.Menus
{
    public interface IModInputMenu : IModMenu
    {
        event Action<string> OnConfirm;
        event Action OnCancel;
        void Initialize(PopupInputMenu inputMenu);
        void Open(InputType inputType, string value);
    }
}
