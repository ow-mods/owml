using System;

namespace OWML.Common.Interfaces.Menus
{
    public interface IModInputMenu : IModTemporaryPopup
    {
        event Action<string> OnConfirm;

        event Action OnCancel;

        void Initialize(PopupInputMenu inputMenu);

        void Open(InputType inputType, string value);

        IModInputMenu Copy();
    }
}
