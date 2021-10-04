using System.Collections.Generic;
using OWML.Common;
using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
	public class ModPopupManager : IModPopupManager
	{
		private readonly IModInputMenu _inputPopup;
		private readonly IModMessagePopup _messagePopup;
		private readonly List<IModTemporaryPopup> _toDestroy = new();
		private readonly IModEvents _events;
		private IModTabbedMenu _options;

		public ModPopupManager(
			IModEvents events,
			IModInputMenu inputPopup,
			IModMessagePopup messagePopup)
		{
			_events = events;
			_inputPopup = inputPopup;
			_messagePopup = messagePopup;
		}

		public void Initialize(PopupInputMenu popupInputMenu, IModTabbedMenu options)
		{
			_options = options;
			var popupCanvas = popupInputMenu.transform.parent.gameObject;
			var newCanvas = GameObject.Instantiate(popupCanvas);
			newCanvas.AddComponent<DontDestroyOnLoad>();

			var inputMenu = newCanvas.GetComponentInChildren<PopupInputMenu>(true);
			var messageMenu = newCanvas.transform.Find("TwoButton-Popup").GetComponent<PopupMenu>();

			_inputPopup.Initialize(inputMenu);
			_messagePopup.Initialize(messageMenu);
		}

		public IModMessagePopup CreateMessagePopup(string message, bool addCancel = false, string okMessage = "OK", string cancelMessage = "Cancel")
		{
			_options.SetIsBlocking(false);
			var newPopup = _messagePopup.Copy();
			_events.Unity.FireOnNextUpdate(() =>
				newPopup.ShowMessage(message, addCancel, okMessage, cancelMessage));
			newPopup.OnCancel += () => OnPopupClose(newPopup);
			newPopup.OnConfirm += () => OnPopupClose(newPopup);
			return newPopup;
		}

		public IModInputMenu CreateInputPopup(InputType inputType, string value)
		{
			_options.SetIsBlocking(false);
			var newPopup = _inputPopup.Copy();
			_events.Unity.FireOnNextUpdate(() =>
				newPopup.Open(inputType, value));
			newPopup.OnCancel += () => OnPopupClose(newPopup);
			newPopup.OnConfirm += _ => OnPopupClose(newPopup);
			return newPopup;
		}

		private void OnPopupClose(IModTemporaryPopup closedPopup)
		{
			_toDestroy.Add(closedPopup);
			_events.Unity.FireOnNextUpdate(CleanUp);
			_options.SetIsBlocking(true);
		}

		private void CleanUp()
		{
			_toDestroy.ForEach(popup => popup.DestroySelf());
			_toDestroy.Clear();
		}
	}
}
