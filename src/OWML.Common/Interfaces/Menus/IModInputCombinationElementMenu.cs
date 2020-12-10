using System;

namespace OWML.Common.Menus
{
    public interface IModInputCombinationElementMenu : IModTemporaryPopup
    {
        event Action<string> OnConfirm;

        event Action OnCancel;

        void Initialize(PopupInputMenu combinationMenu);

        void Init(IModPopupManager popupManager);

        void Open(string value, string comboName, IModInputCombinationMenu combinationMenu, IModInputCombinationElement element);

        IModInputCombinationElementMenu Copy();
    }
}
