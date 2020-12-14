using System;
using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
	public class ModTemporaryPopup : ModMenu, IModTemporaryPopup
	{
		protected PopupMenu Popup;

		public event Action OnCancel;

		protected void DestroySelf(GameObject menuObject)
		{
			GameObject.Destroy(menuObject);
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

		public virtual void DestroySelf() { }

		protected GameObject CopyMenu()
		{
			var newMenuObject = GameObject.Instantiate(Popup.gameObject, Popup.transform.parent);
			newMenuObject.transform.localScale = Popup.transform.localScale;
			newMenuObject.transform.localPosition = Popup.transform.localPosition;
			return newMenuObject;
		}
	}
}
