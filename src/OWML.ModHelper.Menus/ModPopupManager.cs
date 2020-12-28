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
		private readonly IModInputCombinationElementMenu _combinationPopup;
		private readonly List<IModTemporaryPopup> _toDestroy = new();
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

		public void Initialize(PopupInputMenu popupInputMenu)
		{
			var popupCanvas = popupInputMenu.transform.parent.gameObject;
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
			_events.Unity.FireOnNextUpdate(() =>
				newPopup.ShowMessage(message, addCancel, okMessage, cancelMessage));
			newPopup.OnCancel += () => OnPopupClose(newPopup);
			newPopup.OnConfirm += () => OnPopupClose(newPopup);
			return newPopup;
		}

		public IModInputMenu CreateInputPopup(InputType inputType, string value)
		{
			var newPopup = _inputPopup.Copy();
			_events.Unity.FireOnNextUpdate(() =>
				newPopup.Open(inputType, value));
			newPopup.OnCancel += () => OnPopupClose(newPopup);
			newPopup.OnConfirm += _ => OnPopupClose(newPopup);
			return newPopup;
		}

		public IModInputCombinationElementMenu CreateCombinationInput(string value, string comboName,
			IModInputCombinationMenu combinationMenu = null, IModInputCombinationElement element = null)
		{
			var newPopup = _combinationPopup.Copy();
			_events.Unity.FireOnNextUpdate(() =>
				newPopup.Open(value, comboName, combinationMenu, element));
			newPopup.OnCancel += () => OnPopupClose(newPopup);
			newPopup.OnConfirm += _ => OnPopupClose(newPopup);
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
