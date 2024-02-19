using System;
using UnityEngine.UI;
using UnityEngine;
using Steamworks;
using UnityEngine.EventSystems;
using OWML.Common.Interfaces.Menus;
using OWML.Logging;

namespace OWML.ModHelper.Menus.CustomInputs
{
	public class OWMLPopupInputMenu : PopupMenu, IOWMLPopupInputMenu
	{
		public InputField _inputField;
		public InputEventListener _inputFieldEventListener;

		protected Transform _caretTransform;
		protected bool _virtualKeyboardOpen;

		public event PopupInputMenu.InputPopupTextChangedEvent OnInputPopupTextChanged;
		public event PopupInputMenu.InputPopupValidateCharEvent OnInputPopupValidateChar;

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

		public override void SetUpPopup(string message, IInputCommands okCommand, IInputCommands cancelCommand, ScreenPrompt okPrompt, ScreenPrompt cancelPrompt, bool closeMenuOnOk = true, bool setCancelButtonActive = true)
		{
			base.SetUpPopup(message, okCommand, cancelCommand, okPrompt, cancelPrompt, closeMenuOnOk, setCancelButtonActive);
			this._selectOnActivate = this._inputField;
		}

		public override void Activate()
		{
			base.Activate();
			this.ClearInputFieldText();
			this._inputField.ActivateInputField();
			this._inputFieldEventListener.OnSelectEvent += this.OnInputFieldSelect;
			this._inputFieldEventListener.OnPointerUpEvent += this.OnPointerUpInInputField;
			SteamManager.Instance.OnGamepadTextInputDismissed += this.OnSteamVirtualKeyboardDismissed;
			if (SteamManager.Initialized)
			{
				SteamUserStats.RequestCurrentStats();
			}
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
			this._inputField.ActivateInputField();
			return SteamUtils.IsSteamRunningOnSteamDeck() && SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 1280, 370);
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
			this._inputFieldEventListener.OnSelectEvent -= this.OnInputFieldSelect;
			this._inputFieldEventListener.OnPointerUpEvent -= this.OnPointerUpInInputField;
			SteamManager.Instance.OnGamepadTextInputDismissed -= this.OnSteamVirtualKeyboardDismissed;
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
			bool flag = true;
			if (this.OnInputPopupValidateChar != null)
			{
				Delegate[] invocationList = this.OnInputPopupValidateChar.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					bool flag2 = (bool)invocationList[i].DynamicInvoke(new object[] { addedChar });
					flag = flag && flag2;
				}
			}
			if (flag)
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
