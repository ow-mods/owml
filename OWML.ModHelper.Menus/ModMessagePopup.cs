using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace OWML.ModHelper.Menus
{
    public class ModMessagePopup : ModTemporaryPopup, IModMessagePopup
    {
        public event Action OnConfirm;
        public event Action OnCancel;

        private PopupMenu _twoButtonPopup;

        public ModMessagePopup(IModConsole console) : base(console) { }

        internal void Initialize(PopupMenu popup)
        {
            if (Menu != null)
            {
                return;
            }
            _twoButtonPopup = popup;
            if (_twoButtonPopup == null)
            {
                Console.WriteLine("Error: Failed to setup popup");
            }
            Menu = _twoButtonPopup;
        }

        internal override void DestroySelf()
        {
            Object.Destroy(_twoButtonPopup);
            OnConfirm = null;
            OnCancel = null;
            _twoButtonPopup = null;
        }

        internal ModMessagePopup Copy()
        {
            var newPopupObject = Object.Instantiate(_twoButtonPopup.gameObject);
            newPopupObject.transform.SetParent(_twoButtonPopup.transform.parent);
            newPopupObject.transform.localScale = _twoButtonPopup.transform.localScale;
            newPopupObject.transform.localPosition = _twoButtonPopup.transform.localPosition;
            var newPopup = new ModMessagePopup(OwmlConsole);
            newPopup.Initialize(newPopupObject.GetComponent<PopupMenu>());
            return newPopup;
        }

        internal void ShowMessage(string message, bool addCancel = false, string okMessage = "OK", string cancelMessage = "Cancel")
        {
            if (_twoButtonPopup == null)
            {
                OwmlConsole.WriteLine("Failed to create popup for a following message:", MessageType.Warning);
                OwmlConsole.WriteLine(message, MessageType.Info);
            }
            _twoButtonPopup.OnPopupConfirm += OnPopupConfirm;
            _twoButtonPopup.OnPopupCancel += OnPopupCancel;
            _twoButtonPopup.EnableMenu(true);
            var okPrompt = new ScreenPrompt(InputLibrary.confirm, okMessage);
            var cancelPrompt = new ScreenPrompt(InputLibrary.cancel, cancelMessage);
            _twoButtonPopup.SetUpPopup(message, InputLibrary.confirm, addCancel ? InputLibrary.cancel : null,
               okPrompt, cancelPrompt, true, addCancel);
            _twoButtonPopup.GetValue<Text>("_labelText").text = message;
        }

        private void OnPopupConfirm()
        {
            UnregisterEvents();
            OnConfirm?.Invoke();
        }

        private void OnPopupCancel()
        {
            UnregisterEvents();
            OnCancel?.Invoke();
        }

        private void UnregisterEvents()
        {
            _twoButtonPopup.OnPopupConfirm -= OnPopupConfirm;
            _twoButtonPopup.OnPopupCancel -= OnPopupCancel;
        }
    }
}
