using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModMessagePopup : ModMenu, IModMessagePopup
    {
        public event Action OnConfirm;
        public event Action OnCancel;

        private PopupMenu _twoButtonPopup;

        public ModMessagePopup(IModConsole console) : base(console) { }

        public void Initialize(PopupMenu popup)
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

        public void ShowMessage(string message, bool addCancel = false, string okMessage = "OK", string cancelMessage = "Cancel")
        {
            if (_twoButtonPopup == null)
            {
                Console.WriteLine("Failed to create popup for a following message:");
                Console.WriteLine(message);
            }
            _twoButtonPopup.OnPopupConfirm += OnPopupConfirm;
            _twoButtonPopup.OnPopupCancel += OnPopupCancel;
            _twoButtonPopup.EnableMenu(true);
            _twoButtonPopup.SetUpPopup(message, InputLibrary.confirm, addCancel ? InputLibrary.cancel : null,
                new ScreenPrompt(InputLibrary.confirm, okMessage), new ScreenPrompt(InputLibrary.cancel, cancelMessage), true, addCancel);
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
