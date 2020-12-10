using System.Collections.Generic;
using OWML.Common;
using OWML.Common.Interfaces;
using OWML.Common.Interfaces.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModPopupManager : IModPopupManager
    {
        private readonly IModInputMenu _inputPopup;
        private readonly IModMessagePopup _messagePopup;
        private readonly IModInputCombinationElementMenu _combinationPopup;
        private readonly List<IModTemporaryPopup> _toDestroy = new List<IModTemporaryPopup>();
        private readonly IModEvents _events;

        public ModPopupManager(
            IModEvents events,
            IModInputMenu inputPopup,
            IModMessagePopup messagePopup,
            IModInputCombinationElementMenu combinationPopup)
        {
            _events = events;
            _inputPopup = inputPopup;
            _messagePopup = messagePopup;
            _combinationPopup = combinationPopup;

            _combinationPopup.Init(this);
        }

        public void Initialize(GameObject popupCanvas)
        {
            var newCanvas = GameObject.Instantiate(popupCanvas);
            newCanvas.AddComponent<DontDestroyOnLoad>();

            var inputMenu = newCanvas.GetComponentInChildren<PopupInputMenu>(true);
            var combinationMenuObject = GameObject.Instantiate(inputMenu.gameObject);

            combinationMenuObject.transform.SetParent(newCanvas.transform);
            combinationMenuObject.transform.localScale = inputMenu.transform.localScale;
            combinationMenuObject.transform.localPosition = inputMenu.transform.localPosition;

            var combinationMenu = combinationMenuObject.GetComponent<PopupInputMenu>();
            var messageMenu = newCanvas.transform.Find("TwoButton-Popup").GetComponent<PopupMenu>();

            _inputPopup.Initialize(inputMenu);
            _messagePopup.Initialize(messageMenu);
            _combinationPopup.Initialize(combinationMenu);
        }

        public IModMessagePopup CreateMessagePopup(string message, bool addCancel = false, string okMessage = "OK", string cancelMessage = "Cancel")
        {
            var newPopup = _messagePopup.Copy();
            newPopup.ShowMessage(message, addCancel, okMessage, cancelMessage);
            newPopup.OnCancel += () => OnPopupClose(newPopup);
            newPopup.OnConfirm += () => OnPopupClose(newPopup);
            return newPopup;
        }

        public IModInputMenu CreateInputPopup(InputType inputType, string value)
        {
            var newPopup = _inputPopup.Copy();
            newPopup.Open(inputType, value);
            newPopup.OnCancel += () => OnPopupClose(newPopup);
            newPopup.OnConfirm += thing => OnPopupClose(newPopup);
            return newPopup;
        }

        public IModInputCombinationElementMenu CreateCombinationInput(string value, string comboName,
            IModInputCombinationMenu combinationMenu = null, IModInputCombinationElement element = null)
        {
            var newPopup = _combinationPopup.Copy();
            newPopup.Open(value, comboName, combinationMenu, element);
            newPopup.OnCancel += () => OnPopupClose(newPopup);
            newPopup.OnConfirm += thing => OnPopupClose(newPopup);
            return newPopup;
        }

        private void OnPopupClose(IModTemporaryPopup closedPopup)
        {
            _toDestroy.Add(closedPopup);
            _events.Unity.FireOnNextUpdate(CleanUp);
        }

        private void CleanUp()
        {
            _toDestroy.ForEach(popup => popup.DestroySelf());
            _toDestroy.Clear();
        }
    }
}
