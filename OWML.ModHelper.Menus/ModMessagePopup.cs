using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace OWML.ModHelper.Menus
{
    public class ModMessagePopup : ModMenu, IModMessagePopup
    {
        public event Action OnConfirm;
        public event Action OnCancel;
        public bool IsOpen { get; private set; }

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

        public IModMessagePopup Copy()
        {
            var newPopupObject = Object.Instantiate(_twoButtonPopup.gameObject);
            newPopupObject.transform.SetParent(_twoButtonPopup.transform.parent);
            newPopupObject.transform.localScale = _twoButtonPopup.transform.localScale;
            var newPopup = new ModMessagePopup(OwmlConsole);
            newPopup.Initialize(newPopupObject.GetComponent<PopupMenu>());
            return newPopup;
        }

        public void ShowMessage(string message, bool addCancel = false, string okMessage = "OK", string cancelMessage = "Cancel")
        {
            if (_twoButtonPopup == null || IsOpen)
            {
                OwmlConsole.WriteLine("Failed to create popup for a following message:");
                OwmlConsole.WriteLine(message);
            }
            IsOpen = true;
            _twoButtonPopup.OnPopupConfirm += OnPopupConfirm;
            _twoButtonPopup.OnPopupCancel += OnPopupCancel;
            _twoButtonPopup.EnableMenu(true);
            var cancelCommand = addCancel ? InputLibrary.cancel : null;
            var okPrompt = new ScreenPrompt(InputLibrary.confirm, okMessage);
            var cancelPrompt = new ScreenPrompt(InputLibrary.cancel, cancelMessage);
            _twoButtonPopup.SetUpPopup(message, InputLibrary.confirm, cancelCommand, 
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
            IsOpen = false;
        }
    }
}
