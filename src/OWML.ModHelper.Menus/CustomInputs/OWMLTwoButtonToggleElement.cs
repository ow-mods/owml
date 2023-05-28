using OWML.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.CustomInputs
{
	public class OWMLTwoButtonToggleElement : OWMLToggleElement, IMoveHandler, IEventSystemHandler, ISelectHandler, IDeselectHandler
	{
		public Button ButtonTrue;
		public Button ButtonFalse;

		private Button _navigableButton;
		private UIStyleApplier _trueButtonStyleApplier;
		private UIStyleApplier _falseButtonStyleApplier;
		private InputEventListener _trueButtonEventListener;
		private InputEventListener _falseButtonEventListener;

		public new event OptionValueChangedEvent OnValueChanged;
		public new delegate void OptionValueChangedEvent(bool newValue);

		private void Update()
		{
			if (_initOnNextFrame)
			{
				Initialize(_value);
			}
		}

		public override void Initialize(int value)
		{
			base.Initialize(value);
			if (!_initOnNextFrame)
			{
				_initOnNextFrame = true;
				return;
			}

			if (_navigableButton == null)
			{
				_navigableButton = this.GetRequiredComponent<Button>();
			}

			if (ButtonTrue != null)
			{
				_trueButtonStyleApplier = ButtonTrue.GetComponent<UIStyleApplier>();
				_trueButtonEventListener = ButtonTrue.gameObject.GetAddComponent<InputEventListener>();
				_trueButtonStyleApplier.SetAutoInputStateChangesEnabled(false);
				_trueButtonEventListener.OnPointerEnterEvent += OnPointerEnterToggleButton;
				_trueButtonEventListener.OnPointerExitEvent += OnPointerExitToggleButton;
				_trueButtonEventListener.OnPointerUpEvent += OnPointerUpInToggleButton;
			}

			if (ButtonFalse != null)
			{
				_falseButtonStyleApplier = ButtonFalse.GetComponent<UIStyleApplier>();
				_falseButtonEventListener = ButtonFalse.gameObject.GetAddComponent<InputEventListener>();
				_falseButtonStyleApplier.SetAutoInputStateChangesEnabled(false);
				_falseButtonEventListener.OnPointerEnterEvent += OnPointerEnterToggleButton;
				_falseButtonEventListener.OnPointerExitEvent += OnPointerExitToggleButton;
				_falseButtonEventListener.OnPointerUpEvent += OnPointerUpInToggleButton;
			}

			_navigationButtonStyleApplier = base.GetComponent<UIStyleApplier>();
			if (_navigationButtonStyleApplier != null && EventSystem.current.currentSelectedGameObject != base.gameObject)
			{
				_navigationButtonStyleApplier.ChangeState(UIElementState.NORMAL, true);
			}

			UpdateToggleColors();
			_initOnNextFrame = false;
		}

		protected override void OnPointerUpInToggleButton(PointerEventData eventData, Selectable selectable)
		{
			var toggleState = (ToggleElement.ToggleState)_value;
			if (selectable.gameObject == ButtonTrue.gameObject && eventData.pointerPress == ButtonTrue.gameObject)
			{
				toggleState = ToggleElement.ToggleState.STATE_TRUE;
			}
			else if (selectable.gameObject == ButtonFalse.gameObject && eventData.pointerPress == ButtonFalse.gameObject)
			{
				toggleState = ToggleElement.ToggleState.STATE_FALSE;
			}

			if (toggleState != (ToggleElement.ToggleState)_value)
			{
				_value = (int)toggleState;
				Locator.GetMenuAudioController().PlayOptionToggle();
				UpdateToggleColors();
				OnValueChanged?.Invoke(toggleState == ToggleElement.ToggleState.STATE_TRUE);
			}

			Locator.GetMenuInputModule().SelectOnNextUpdate(_navigableButton);
		}

		protected override void OnPointerExitToggleButton(PointerEventData eventData, Selectable selectable)
		{
			UpdateToggleColors();
		}

		protected override void OnPointerEnterToggleButton(PointerEventData eventData, Selectable selectable)
		{
			if (selectable.gameObject == ButtonTrue.gameObject)
			{
				_trueButtonStyleApplier.ChangeState(UIElementState.ROLLOVER_HIGHLIGHT, false);
				return;
			}

			if (selectable.gameObject == ButtonFalse.gameObject)
			{
				_falseButtonStyleApplier.ChangeState(UIElementState.ROLLOVER_HIGHLIGHT, false);
			}
		}

		private void VirtualSelect(AxisEventData eventData)
		{
			if (_navigableButton == null)
			{
				return;
			}

			var navigation = _navigableButton.navigation;
			var flag = false;
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					if (navigation.selectOnLeft != null)
					{
						flag = true;
					}

					break;
				case MoveDirection.Up:
					if (navigation.selectOnUp != null)
					{
						flag = true;
					}

					break;
				case MoveDirection.Right:
					if (navigation.selectOnRight != null)
					{
						flag = true;
					}

					break;
				case MoveDirection.Down:
					if (navigation.selectOnDown != null)
					{
						flag = true;
					}

					break;
			}

			if (flag)
			{
				return;
			}

			var toggleState = (ToggleElement.ToggleState)_value;
			Button button;
			Navigation navigation2;
			ToggleElement.ToggleState toggleState2;
			if (toggleState == ToggleElement.ToggleState.STATE_TRUE)
			{
				button = ButtonFalse;
				navigation2 = ButtonTrue.navigation;
				toggleState2 = ToggleElement.ToggleState.STATE_FALSE;
			}
			else
			{
				button = ButtonTrue;
				navigation2 = ButtonFalse.navigation;
				toggleState2 = ToggleElement.ToggleState.STATE_TRUE;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					if (navigation2.selectOnLeft == button)
					{
						toggleState = toggleState2;
					}

					break;
				case MoveDirection.Up:
					if (navigation2.selectOnUp == button)
					{
						toggleState = toggleState2;
					}

					break;
				case MoveDirection.Right:
					if (navigation2.selectOnRight == button)
					{
						toggleState = toggleState2;
					}

					break;
				case MoveDirection.Down:
					if (navigation2.selectOnDown == button)
					{
						toggleState = toggleState2;
					}

					break;
			}

			if (_value != (int)toggleState)
			{
				_value = (int)toggleState;
				OnValueChanged?.Invoke(toggleState == ToggleElement.ToggleState.STATE_TRUE);
				Locator.GetMenuAudioController().PlayOptionToggle();
			}
		}

		public void OnMove(AxisEventData eventData)
		{
			VirtualSelect(eventData);
			UpdateToggleColors();
		}

		protected override void UpdateToggleColors()
		{
			if (_trueButtonStyleApplier == null || _falseButtonStyleApplier == null)
			{
				return;
			}

			UIStyleApplier uistyleApplier;
			UIStyleApplier uistyleApplier2;
			if (_value == 1)
			{
				uistyleApplier = _trueButtonStyleApplier;
				uistyleApplier2 = _falseButtonStyleApplier;
			}
			else
			{
				uistyleApplier = _falseButtonStyleApplier;
				uistyleApplier2 = _trueButtonStyleApplier;
			}

			if (uistyleApplier != null)
			{
				if (_uiElementSelected)
				{
					uistyleApplier.ChangeState(UIElementState.HIGHLIGHTED, false);
				}
				else
				{
					uistyleApplier.ChangeState(UIElementState.INTERMEDIATELY_HIGHLIGHTED, false);
				}
			}

			if (uistyleApplier2 != null)
			{
				uistyleApplier2.ChangeState(UIElementState.NORMAL, false);
			}
		}

	}
}
