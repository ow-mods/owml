﻿using System;

namespace OWML.Common.Menus
{
    public interface IModMessagePopup : IModMenu
    {
        event Action OnConfirm;
        event Action OnCancel;

        void Initialize(PopupMenu popup);
        void ShowMessage(string message, bool addCancel = false, string okMessage = "OK", string cancelMessage = "Cancel");
    }
}