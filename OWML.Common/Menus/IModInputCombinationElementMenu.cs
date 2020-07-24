﻿using System;

namespace OWML.Common.Menus
{
    public interface IModInputCombinationElementMenu : IModMenu
    {
        event Action<string> OnConfirm;
        event Action OnCancel;
        IModMessagePopup MessagePopup { get; }

        void Initialize(PopupInputMenu menu);
        void Open(string value, string comboName, IModInputCombinationMenu combinationMenu = null,
            IModInputCombinationElement element = null);
    }
}
