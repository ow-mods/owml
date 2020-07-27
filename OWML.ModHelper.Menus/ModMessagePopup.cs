using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModMessagePopup : ModTemporaryPopup, IModMessagePopup
    {
        public event Action OnConfirm;

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
            Popup = _twoButtonPopup;
        }

        internal override void DestroySelf()
        {
            DestroySelf(_twoButtonPopup.gameObject);
            OnConfirm = null;
            _twoButtonPopup = null;
        }

        internal ModMessagePopup Copy()
        {
            var newPopupObject = CopyMenu();
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
            RegisterEvents();
            _twoButtonPopup.EnableMenu(true);
            var okPrompt = new ScreenPrompt(InputLibrary.confirm, okMessage);
            var cancelPrompt = new ScreenPrompt(InputLibrary.cancel, cancelMessage);
            _twoButtonPopup.SetUpPopup(message, InputLibrary.confirm, addCancel ? InputLibrary.cancel : null,
               okPrompt, cancelPrompt, true, addCancel);
            _twoButtonPopup.GetValue<Text>("_labelText").text = message;
        }

        protected override void OnPopupConfirm()
        {
            base.OnPopupConfirm();
            OnConfirm?.Invoke();
        }
    }
}
