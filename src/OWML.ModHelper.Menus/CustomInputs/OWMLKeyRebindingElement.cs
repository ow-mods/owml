using OWML.Common;
using OWML.Common.Interfaces.Menus;
using OWML.ModHelper.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.CustomInputs
{
	[RequireComponent(typeof(Selectable))]
	public class OWMLKeyRebindingElement : MenuOption, IEventSystemHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IMoveHandler, ICancelHandler, IOWMLKeyRebindingElement
	{
		public RebindableID _rebindID;
		public Button _controlButton;
		public SubmitAction _controlSubmitAction;
		public Button _labelButton;
		public float _referenceButtonImageHeight;
		public GameObject _gamepadBindingImage1Obj;
		public GameObject _gamepadBindingImage2Obj;
		public Image _gamepadBindingImage1;
		public Image _gamepadBindingImage2;
		public GameObject _keyboardMouseBindingBlockObj;
		public GameObject _keyboardMouseBindingImage1Obj;
		public GameObject _keyboardMouseBindingImage2Obj;
		public Image _keyboardMouseBindingImage1;
		public Image _keyboardMouseBindingImage2;

		public IModConsole _console;

		protected LayoutElement _gamepadBindingImage1Layout;
		protected LayoutElement _gamepadBindingImage2Layout;
		protected LayoutElement _keyboardMouseBindingImage1Layout;
		protected LayoutElement _keyboardMouseBindingImage2Layout;
		protected Texture2D[] _gamepadBindingTextures;
		protected Texture2D[] _keyboardMouseBindingTextures;
		protected Selectable _rebindingElementSelectable;
		protected UIStyleApplier _controlButtonStyleApplier;
		protected InputEventListener _controlButtonEventListener;
		protected SettingsMenuModel _settingsModel;
		protected IRebindableInputAction _rebindingInputCommand;
		protected IInputCommands _cancelRebind1;
		protected IInputCommands _cancelRebind2;
		protected RebindingState _rebindState;
		protected KeyRebindingElement.RebindingElementState _currentState;

		protected void OnValidate()
		{
			_settingId = SettingsID.REBINDABLE_OPTION;
		}

		public virtual void Initialize(SettingsMenuModel settingsMenuModel)
		{
			base.Initialize();
			_rebindingElementSelectable = this.GetRequiredComponent<Selectable>();
			if (!gameObject.activeSelf)
			{
				return;
			}
			_keyboardMouseBindingTextures = new Texture2D[2];
			_settingsModel = settingsMenuModel;
			_gamepadBindingTextures = new Texture2D[2];
			_gamepadBindingImage1Layout = _gamepadBindingImage1Obj.GetRequiredComponent<LayoutElement>();
			_gamepadBindingImage2Layout = _gamepadBindingImage2Obj.GetRequiredComponent<LayoutElement>();
			_keyboardMouseBindingImage1Layout = _keyboardMouseBindingImage1Obj.GetRequiredComponent<LayoutElement>();
			_keyboardMouseBindingImage2Layout = _keyboardMouseBindingImage2Obj.GetRequiredComponent<LayoutElement>();
			IInputCommands inputCommands = null;
			InputCommandDefinitions.InputCommandData inputCommandData;
			InputCommandDefinitions.TryGetInputCommandData(_rebindID, out inputCommandData);
			var commandType = inputCommandData.CommandType;
			InputCommandManager.MappedInputActions.TryGetValue(commandType, out inputCommands);
			if (inputCommands == null || !inputCommands.IsRebindable)
			{
				Debug.LogError("Command is non-bindable!", gameObject);
				return;
			}
			InputCommands inputCommands2;
			IRebindableInputAction rebindableInputAction;
			InputAxisCommands inputAxisCommands;
			IRebindableInputAction rebindableInputAction2;
			CompositeInputCommands compositeInputCommands;
			if ((inputCommands2 = inputCommands as InputCommands) != null && (rebindableInputAction = inputCommands2.Action as IRebindableInputAction) != null)
			{
				SetRebindableAction(rebindableInputAction);
			}
			else if ((inputAxisCommands = inputCommands as InputAxisCommands) != null && (rebindableInputAction2 = inputAxisCommands.Action as IRebindableInputAction) != null)
			{
				SetRebindableAction(rebindableInputAction2);
			}
			else if ((compositeInputCommands = inputCommands as CompositeInputCommands) != null)
			{
				var actionForRebindID = compositeInputCommands.GetActionForRebindID(_rebindID);
				SetRebindableAction(actionForRebindID);
			}
			_cancelRebind1 = InputLibrary.cancelRebinding1;
			_cancelRebind2 = InputLibrary.cancelRebinding2;
			if (_labelButton != null)
			{
				_labelButton.onClick.AddListener(new UnityAction(OnPointerUpInLabelButton));
			}
			_controlButtonStyleApplier = _controlButton.GetComponent<UIStyleApplier>();
			_controlButtonStyleApplier.SetAutoInputStateChangesEnabled(false);
			_controlButtonStyleApplier.ChangeState(UIElementState.NORMAL, false);
			_controlButtonEventListener = _controlButton.gameObject.GetAddComponent<InputEventListener>();
			_controlButtonEventListener.OnPointerEnterEvent += OnPointerEnterKeyBinderButton;
			_controlButtonEventListener.OnPointerExitEvent += OnPointerExitKeyBinderButton;
			_controlButtonEventListener.OnPointerUpEvent += OnPointerUpInKeyBinderButton;
			UpdateDisplay(false);
		}

		protected virtual void EnterRebindingState()
		{
			if (_currentState == KeyRebindingElement.RebindingElementState.SELECTED)
			{
				OWInput.ChangeInputMode(InputMode.Rebinding);
				//Debug.Log("ENTER REBINDING MODE: " + _rebindID.ToString(), this);
				_console.WriteLine("ENTER REBINDING MODE: " + _rebindID.ToString());
				_currentState = KeyRebindingElement.RebindingElementState.REBINDING;
				UpdateRebindingButtonColors();
				RebindingState rebindingState;
				if (_settingsModel.InitializeRebindingAction(_rebindingInputCommand, out rebindingState))
				{
					_rebindState = rebindingState;
					var rebindState = _rebindState;
					rebindState.OnFinishedRebinding = (Action)Delegate.Combine(rebindState.OnFinishedRebinding, new Action(OnFinishedRebinding));
					var rebindState2 = _rebindState;
					rebindState2.OnCancelledRebinding = (Action)Delegate.Combine(rebindState2.OnCancelledRebinding, new Action(OnCancelledRebinding));
					return;
				}
				ExitRebindingState();
			}
		}

		protected void ExitRebindingState()
		{
			_rebindState = null;
			if (_currentState == KeyRebindingElement.RebindingElementState.REBINDING)
			{
				_currentState = KeyRebindingElement.RebindingElementState.SELECTED;
				Locator.GetMenuInputModule().SelectOnNextUpdate(_rebindingElementSelectable);
				UpdateRebindingButtonColors();
				OWInput.RestorePreviousInputs();
			}
		}

		protected virtual void OnFinishedRebinding()
		{
			_console.WriteLine("Rebind COMPLETE");
			ExitRebindingState();
		}

		protected void OnCancelledRebinding()
		{
			_console.WriteLine("Rebind CANCELED");
			ExitRebindingState();
			UpdateDisplay(false);
		}

		public RebindableID GetRebindableID()
		{
			return _rebindID;
		}

		protected void OnRebindingMenuClose()
		{
			_console.WriteLine("OnRebindingMenuClose");
			_currentState = KeyRebindingElement.RebindingElementState.STANDBY;
			_controlButton.GetComponent<UIStyleApplier>().ChangeState(UIElementState.NORMAL, false);
		}

		protected void OnDestroy()
		{
			_console.WriteLine("OnDestroy");

			if (_labelButton != null)
			{
				_labelButton.onClick.RemoveListener(new UnityAction(OnPointerUpInLabelButton));
			}

			OWMLRebinding.ListCustomRebindableOptions.Remove(this);
		}

		public void SetRebindableAction(IRebindableInputAction rebindableAction)
		{
			_rebindingInputCommand = rebindableAction;
			UpdateDisplay(false);
		}

		public bool IsInRebindingMode()
		{
			return _currentState == KeyRebindingElement.RebindingElementState.REBINDING;
		}

		public virtual void UpdateDisplay(bool forceRefresh = false)
		{
			if (_rebindingInputCommand == null)
			{
				return;
			}
			UpdateTextures(true, _gamepadBindingImage1, _gamepadBindingImage2, _gamepadBindingImage1Layout, _gamepadBindingImage2Layout, forceRefresh);
			UpdateTextures(false, _keyboardMouseBindingImage1, _keyboardMouseBindingImage2, _keyboardMouseBindingImage1Layout, _keyboardMouseBindingImage2Layout, forceRefresh);
		}

		protected virtual void UpdateTextures(bool usingGamepad, Image image1, Image image2, LayoutElement layout1, LayoutElement layout2, bool forceRefresh)
		{
			List<Texture2D> list;
			if (_rebindState != null)
			{
				list = _rebindState.GetCurrentlyBindingAction().GetUITextures(usingGamepad, forceRefresh, false);
			}
			else
			{
				list = _rebindingInputCommand.GetUITextures(usingGamepad, forceRefresh, false);
			}
			if (list.Count > 0)
			{
				image1.sprite = Sprite.Create(list[0], new Rect(0f, 0f, (float)list[0].width, (float)list[0].height), new Vector2(0.5f, 0.5f), (float)list[0].width);
				image1.enabled = true;
				var vector = ButtonPromptLibrary.AdjustButtonImageSize(list[0], _referenceButtonImageHeight, true);
				layout1.minHeight = vector.y;
			}
			else
			{
				image1.enabled = false;
			}
			var gameObject = (usingGamepad ? _gamepadBindingImage2Obj : _keyboardMouseBindingImage2Obj);
			if (list.Count > 1)
			{
				if (list[0] == list[1])
				{
					gameObject.SetActive(false);
					return;
				}
				gameObject.SetActive(true);
				image2.sprite = Sprite.Create(list[1], new Rect(0f, 0f, (float)list[1].width, (float)list[1].height), new Vector2(0.5f, 0.5f), (float)list[1].width);
				var vector2 = ButtonPromptLibrary.AdjustButtonImageSize(list[1], _referenceButtonImageHeight, true);
				layout2.minHeight = vector2.y;
				if (RebindableLookup.IsXAxisRebindable(_rebindID))
				{
					var sprite = image1.sprite;
					image1.sprite = image2.sprite;
					image2.sprite = sprite;
					return;
				}
			}
			else
			{
				gameObject.SetActive(false);
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (_currentState == KeyRebindingElement.RebindingElementState.STANDBY)
			{
				_currentState = KeyRebindingElement.RebindingElementState.SELECTED;
			}
			UpdateRebindingButtonColors();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			if (_currentState == KeyRebindingElement.RebindingElementState.SELECTED)
			{
				PointerEventData pointerEventData;
				if ((pointerEventData = eventData as PointerEventData) != null && pointerEventData.pointerPressRaycast.gameObject.transform.IsChildOf(transform))
				{
					_selectable.Select();
					return;
				}
				_currentState = KeyRebindingElement.RebindingElementState.STANDBY;
				_controlButton.GetComponent<UIStyleApplier>().ChangeState(UIElementState.NORMAL, false);
			}
		}

		public void OnMove(AxisEventData eventData)
		{
			if (_currentState > KeyRebindingElement.RebindingElementState.SELECTED)
			{
				eventData.Use();
			}
		}

		public void OnCancel(BaseEventData eventData)
		{
			_controlButtonStyleApplier.ChangeState(UIElementState.NORMAL, false);
		}

		public void OnSubmit(BaseEventData eventData)
		{
			eventData.Use();
			if (_currentState == KeyRebindingElement.RebindingElementState.SELECTED)
			{
				_controlButtonStyleApplier.ChangeState(UIElementState.HIGHLIGHTED, false);
				EnterRebindingState();
			}
		}

		protected virtual void OnPointerUpInLabelButton()
		{
			if (_currentState == KeyRebindingElement.RebindingElementState.STANDBY)
			{
				_rebindingElementSelectable.Select();
				return;
			}
			if (_currentState == KeyRebindingElement.RebindingElementState.SELECTED)
			{
				_controlButtonStyleApplier.ChangeState(UIElementState.HIGHLIGHTED, false);
				EnterRebindingState();
				Locator.GetMenuAudioController().PlayOptionToggle();
			}
		}

		protected virtual void OnPointerUpInKeyBinderButton(PointerEventData eventData, Selectable selectable)
		{
			_console.WriteLine("Pointer Up!");
			if (_currentState == KeyRebindingElement.RebindingElementState.STANDBY)
			{
				eventData.Use();
				_rebindingElementSelectable.Select();
				return;
			}
			if (_currentState == KeyRebindingElement.RebindingElementState.SELECTED)
			{
				eventData.Use();
				Locator.GetMenuAudioController().PlayOptionToggle();
				_rebindingElementSelectable.Select();
				EnterRebindingState();
			}
		}

		protected virtual void OnPointerExitKeyBinderButton(PointerEventData eventData, Selectable selectable)
		{
			UpdateRebindingButtonColors();
		}

		protected virtual void OnPointerEnterKeyBinderButton(PointerEventData eventData, Selectable selectable)
		{
			_controlButtonStyleApplier.ChangeState(UIElementState.HIGHLIGHTED, false);
		}

		protected void UpdateRebindingButtonColors()
		{
			var currentState = _currentState;
			_console.WriteLine($"currentState: {currentState}");
			UIElementState uielementState;
			if (currentState != KeyRebindingElement.RebindingElementState.SELECTED)
			{
				if (currentState == KeyRebindingElement.RebindingElementState.REBINDING)
				{
					uielementState = UIElementState.INTERMEDIATELY_HIGHLIGHTED;
				}
				else
				{
					uielementState = UIElementState.NORMAL;
				}
			}
			else
			{
				uielementState = UIElementState.HIGHLIGHTED;
			}

			_console.WriteLine($"uielementState: {uielementState}");

			_controlButtonStyleApplier.ChangeState(uielementState, false);
		}
	}
}
