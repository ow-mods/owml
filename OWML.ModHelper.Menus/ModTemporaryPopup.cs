using System;
using OWML.Common;
using Object = UnityEngine.Object;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModTemporaryPopup : ModMenu
    {
        protected PopupMenu Popup;

        public event Action OnCancel;
        public ModTemporaryPopup(IModConsole console) : base(console) { }

        protected void DestroySelf(GameObject menuObject)
        {
            Object.Destroy(menuObject);
            OnCancel = null;
        }

        protected virtual void RegisterEvents()
        {
            Popup.OnPopupCancel += OnPopupCancel;
            Popup.OnPopupConfirm += OnPopupConfirm;
        }

        protected virtual void UnregisterEvents()
        {
            Popup.OnPopupCancel -= OnPopupCancel;
            Popup.OnPopupConfirm -= OnPopupConfirm;
        }

        protected virtual void OnPopupCancel()
        {
            UnregisterEvents();
            OnCancel?.Invoke();
        }

        protected virtual void OnPopupConfirm()
        {
            UnregisterEvents();
        }

        internal virtual void DestroySelf() { }

        protected GameObject CopyMenu()
        {
            var newMenuObject = Object.Instantiate(Popup.gameObject, Popup.transform.parent);
            newMenuObject.transform.localScale = Popup.transform.localScale;
            newMenuObject.transform.localPosition = Popup.transform.localPosition;
            return newMenuObject;
        }
    }
}
