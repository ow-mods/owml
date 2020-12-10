using System;

namespace OWML.Common.Menus
{
    public interface IModMessagePopup : IModTemporaryPopup
    {
        event Action OnConfirm;

        event Action OnCancel;

        void Initialize(PopupMenu messageMenu);

        void ShowMessage(string message, bool addCancel, string okMessage, string cancelMessage);

        IModMessagePopup Copy();
    }
}
