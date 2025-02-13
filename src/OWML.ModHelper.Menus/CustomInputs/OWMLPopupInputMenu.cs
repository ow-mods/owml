using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using OWML.Common.Interfaces.Menus;

namespace OWML.ModHelper.Menus.CustomInputs
{
	public class OWMLPopupInputMenu : PopupMenu, IOWMLPopupInputMenu
	{
		public InputField _inputField;
		public InputEventListener _inputFieldEventListener;

		protected Transform _caretTransform;
		protected bool _virtualKeyboardOpen;

		public event PopupInputMenu.InputPopupTextChangedEvent OnInputPopupTextChanged;
		public event IOWMLPopupInputMenu.InputPopupValidateCharEvent OnValidateChar;

		[Obsolete("Use OnValidateChar instead.")]
		public event PopupInputMenu.InputPopupValidateCharEvent OnInputPopupValidateChar;

		private bool _setCancelButtonActive;

		public override void Awake()
		{
			base.Awake();
			this._inputField.DeactivateInputField();
		}

		public override void InitializeMenu()
		{
			base.InitializeMenu();

			// PopupCanvas is disabled after the menus are initialized, and overrideSorting can only be set when it's enabled
			_menuActivationRoot.gameObject.SetActive(true);
			_popupCanvas = gameObject.GetAddComponent<Canvas>();
			_popupCanvas.overrideSorting = true;
			_popupCanvas.sortingOrder = 30000;
			_menuActivationRoot.gameObject.SetActive(false);
		}

		public override Selectable GetSelectOnActivate()
		{
			return this._selectOnActivate;
		}

		public void SetText(string message, string placeholderMessage, string confirmText, string cancelText)
		{
			var okPrompt = _confirmButton._screenPrompt == null ? null : new ScreenPrompt(_confirmButton._screenPrompt._commandList[0], confirmText);
			var cancelPrompt = _cancelButton._screenPrompt == null ? null : new ScreenPrompt(_cancelButton._screenPrompt._commandList[0], cancelText);
			SetUpPopup(
				message,
				_okCommand,
				_cancelCommand,
				okPrompt,
				cancelPrompt,
				_closeMenuOnOk,
				_setCancelButtonActive);
			SetInputFieldPlaceholderText(placeholderMessage);
		}

		public override void SetUpPopup(string message, IInputCommands okCommand, IInputCommands cancelCommand, ScreenPrompt okPrompt, ScreenPrompt cancelPrompt, bool closeMenuOnOk = true, bool setCancelButtonActive = true)
		{
			_closeMenuOnOk = closeMenuOnOk;
			_setCancelButtonActive = setCancelButtonActive;
			base.SetUpPopup(message, okCommand, cancelCommand, okPrompt, cancelPrompt, closeMenuOnOk, setCancelButtonActive);
			this._selectOnActivate = this._inputField;
		}

		public override void Activate()
		{
			base.Activate();
			this.ClearInputFieldText();
			this._inputField.ActivateInputField();
			this._inputField.onValueChanged.AddListener(delegate
			{
				this.OnTextFieldChanged();
			});
			InputField inputField = this._inputField;
			inputField.onValidateInput += this.OnValidateInput;
			Transform transform = this._inputField.transform.Find(this._inputField.transform.name + " Input Caret");
			if (transform != null)
			{
				this._caretTransform = transform;
				transform.SetAsLastSibling();
			}
		}

		protected void OnInputFieldSelect(BaseEventData eventData, Selectable selectable)
		{
			if (selectable != this._inputField)
			{
				return;
			}
			this._virtualKeyboardOpen = this.TryOpenVirtualKeyboard();
		}

		protected void OnPointerUpInInputField(PointerEventData eventData, Selectable selectable)
		{
			this._virtualKeyboardOpen = this.TryOpenVirtualKeyboard();
		}

		protected bool TryOpenVirtualKeyboard()
		{
			return false;
		}

		protected void OnSteamVirtualKeyboardDismissed(bool bSubmitted, uint unSubmittedText)
		{
			this._virtualKeyboardOpen = false;
		}

		public override void InvokeOk()
		{
			base.InvokeOk();
			if (this._enabledMenu && this._inputField.text == "")
			{
				if (Locator.GetEventSystem().currentSelectedGameObject != this._inputField.gameObject)
				{
					this._inputField.Select();
					return;
				}
				this._virtualKeyboardOpen = this.TryOpenVirtualKeyboard();
			}
		}

		public override void Update()
		{
			if (this._caretTransform == null)
			{
				Transform transform = this._inputField.transform.Find(this._inputField.transform.name + " Input Caret");
				if (transform != null)
				{
					this._caretTransform = transform;
					transform.SetAsLastSibling();
				}
			}
			if (OWInput.IsNewlyPressed(InputLibrary.menuConfirm, InputMode.All))
			{
				if (Locator.GetEventSystem().currentSelectedGameObject == this._inputField.gameObject)
				{
					this._virtualKeyboardOpen = this.TryOpenVirtualKeyboard();
					return;
				}
				EventSystem eventSystem = Locator.GetEventSystem();
				if (eventSystem.currentSelectedGameObject != this._confirmButton.gameObject && eventSystem.currentSelectedGameObject != this._cancelButton.gameObject)
				{
					this._inputField.Select();
				}
			}
		}

		public override void Deactivate(bool keepPreviousMenuVisible = false)
		{
			base.Deactivate(keepPreviousMenuVisible);
			this._inputField.DeactivateInputField();
			this._inputField.onValueChanged.RemoveListener(delegate
			{
				this.OnTextFieldChanged();
			});
			InputField inputField = this._inputField;
			inputField.onValidateInput = (InputField.OnValidateInput)Delegate.Remove(inputField.onValidateInput, new InputField.OnValidateInput(this.OnValidateInput));
			this._virtualKeyboardOpen = false;
		}

		private void OnTextFieldChanged()
		{
			if (this.OnInputPopupTextChanged != null)
			{
				this.OnInputPopupTextChanged();
			}
		}

		private char OnValidateInput(string input, int charIndex, char addedChar)
		{
			var isValidCharacter = true;
			if (OnInputPopupValidateChar != null)
			{
				var invocationList = OnInputPopupValidateChar.GetInvocationList();
				for (var i = 0; i < invocationList.Length; i++)
				{
					var flag2 = (bool)invocationList[i].DynamicInvoke(new object[] { addedChar });
					isValidCharacter = isValidCharacter && flag2;
				}
			}

			if (OnValidateChar != null && isValidCharacter)
			{
				var invocationList = OnValidateChar.GetInvocationList();
				for (var i = 0; i < invocationList.Length; i++)
				{
					var flag2 = (bool)invocationList[i].DynamicInvoke(new object[] { input, charIndex, addedChar });
					isValidCharacter = isValidCharacter && flag2;
				}
			}

			if (isValidCharacter)
			{
				return addedChar;
			}

			return '\0';
		}

		public void ClearInputFieldText()
		{
			this._inputField.text = "";
		}

		public virtual string GetInputText()
		{
			return this._inputField.text;
		}

		public virtual InputField GetInputField()
		{
			return this._inputField;
		}

		public virtual void SetInputFieldPlaceholderText(string text)
		{
			Text component = this._inputField.placeholder.GetComponent<Text>();
			if (component != null)
			{
				component.text = text;
				return;
			}
			Debug.LogWarning("Could not find InputField Placeholder Text Element");
		}

		public override void SetUpPopupCommands(IInputCommands okCommand, IInputCommands cancelCommand, ScreenPrompt okPrompt, ScreenPrompt cancelPrompt)
		{
			this._okCommand = okCommand;
			this._cancelCommand = cancelCommand;
			this._confirmButton.SetPrompt(okPrompt, InputMode.Menu);
			this._cancelButton.SetPrompt(cancelPrompt, InputMode.Menu);
		}
	}
}
