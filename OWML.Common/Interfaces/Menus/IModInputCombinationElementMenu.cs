using System;

namespace OWML.Common.Interfaces.Menus
{
    public interface IModInputCombinationElementMenu : IModTemporaryPopup
    {
        event Action<string> OnConfirm;

        event Action OnCancel;

        void Initialize(PopupInputMenu combinationMenu);

        void Open(string value, string comboName, IModInputCombinationMenu combinationMenu, IModInputCombinationElement element);

        IModInputCombinationElementMenu Copy();
    }
}
