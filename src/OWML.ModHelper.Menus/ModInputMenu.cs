using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
	public class ModInputMenu : ModTemporaryPopup, IModInputMenu
	{
		public event Action<string> OnConfirm;

		private PopupInputMenu _inputMenu;

		public ModInputMenu(IModConsole console)
			: base(console)
		{
		}

		public void Initialize(PopupInputMenu menu)
		{
			if (Menu != null)
			{
				return;
			}
			Popup = menu;
			var parent = menu.transform.parent.gameObject;
			var parentCopy = GameObject.Instantiate(parent);
			parentCopy.AddComponent<DontDestroyOnLoad>();
			_inputMenu = parentCopy.transform.GetComponentInChildren<PopupInputMenu>(true);
			GameObject.Destroy(_inputMenu.GetValue<Text>("_labelText").GetComponent<LocalizedText>());
			Initialize((Menu)_inputMenu);
		}

		public void Open(InputType inputType, string value)
		{
			RegisterEvents();
			if (inputType == InputType.Number)
			{
				_inputMenu.OnInputPopupValidateChar += OnValidateCharNumber;
				_inputMenu.OnPopupValidate += OnValidateNumber;
			}
			var message = inputType == InputType.Number ? "Write a number" : "Write some text";

			_inputMenu.EnableMenu(true);

			var okPrompt = new ScreenPrompt(InputLibrary.confirm2, "OK");
			var cancelCommand = OWInput.UsingGamepad() ? InputLibrary.cancel : InputLibrary.escape;
			var cancelPrompt = new ScreenPrompt(cancelCommand, "Cancel");
			_inputMenu.SetUpPopup(message, InputLibrary.confirm2, cancelCommand, okPrompt, cancelPrompt);
			_inputMenu.SetInputFieldPlaceholderText("");
			_inputMenu.GetInputField().text = value;
			_inputMenu.GetValue<Text>("_labelText").text = message;
		}

		public IModInputMenu Copy()
		{
			var newPopupObject = CopyMenu();
			var newPopup = new ModInputMenu(Console);
			newPopup.Initialize(newPopupObject.GetComponent<PopupInputMenu>());
			newPopup._inputMenu.SetValue("_initialized", false);
			return newPopup;
		}

		public override void DestroySelf()
		{
			_inputMenu.EnableMenu(false);
			DestroySelf(_inputMenu.gameObject);
			OnConfirm = null;
			_inputMenu = null;
		}

		private bool OnValidateNumber() =>
			float.TryParse(_inputMenu.GetInputText(), out _);

		private bool OnValidateCharNumber(char c) =>
			"0123456789.".Contains("" + c);

		protected override void OnPopupConfirm()
		{
			base.OnPopupConfirm();
			OnConfirm?.Invoke(_inputMenu.GetInputText());
		}

		protected override void RegisterEvents()
		{
			_inputMenu.OnPopupCancel += OnPopupCancel; // subscribing to PopupMenu doesn't work *shrug*
			_inputMenu.OnPopupConfirm += OnPopupConfirm;
		}

		protected override void UnregisterEvents()
		{
			_inputMenu.OnPopupCancel -= OnPopupCancel;
			_inputMenu.OnPopupConfirm -= OnPopupConfirm;
			_inputMenu.OnPopupValidate -= OnValidateNumber;
			_inputMenu.OnInputPopupValidateChar -= OnValidateCharNumber;
		}
	}
}
