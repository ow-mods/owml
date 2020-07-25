using UnityEngine;

namespace OWML.Common.Menus
{
    public interface IModPopupManager
    {
        void Initialize(GameObject popupCanvas);
        IModMessagePopup CreateMessage(string message, bool addCancel = false, string okMessage = "OK", string cancelMessage = "Cancel");
        IModInputMenu CreateInput(InputType inputType, string value);
        IModInputCombinationElementMenu CreateCombinationInput(string value, string comboName, 
            IModInputCombinationMenu combinationMenu = null, IModInputCombinationElement element = null);
    }
}
