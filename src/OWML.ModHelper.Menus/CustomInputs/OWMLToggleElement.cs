using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OWML.ModHelper.Menus.CustomInputs
{
	public class OWMLToggleElement : OWMLMenuValueOption, ISelectHandler, IEventSystemHandler, IDeselectHandler, ISubmitHandler, ICancelHandler
	{
		public Text _displayText;
		public Graphic _toggleGraphic;
		public Button _toggleElementButton;

		protected InputEventListener _buttonEventListener;
		protected UIStyleApplier _navigationButtonStyleApplier;
		protected bool _initOnNextFrame;
		protected bool _uiElementSelected;
		private bool _mouseClickInElement;

		public delegate void ElementUIEvent(BaseEventData eventData, OWMLToggleElement selectable);
		public delegate void ToggleEvent(OWMLToggleElement selectable);

		private void Update()
		{
			if (_initOnNextFrame)
			{
				Initialize(_value);
			}
		}

		public override void Initialize(int value)
		{
			if (value < 0 || value > 1)
			{
				Debug.LogError("Initialize value for " + base.gameObject.name + " is not 0 or 1");
			}
			base.Initialize(value);
			if (!_initOnNextFrame)
			{
				_initOnNextFrame = true;
				return;
			}
			if (_toggleElementButton == null)
			{
				_toggleElementButton = this.GetRequiredComponent<Button>();
			}
			if (_toggleElementButton != null)
			{
				_navigationButtonStyleApplier = _toggleElementButton.GetComponent<UIStyleApplier>();
				_buttonEventListener = _toggleElementButton.gameObject.GetAddComponent<InputEventListener>();
				_navigationButtonStyleApplier.SetAutoInputStateChangesEnabled(false);
				_buttonEventListener.OnPointerEnterEvent += OnPointerEnterToggleButton;
				_buttonEventListener.OnPointerExitEvent += OnPointerExitToggleButton;
				_buttonEventListener.OnPointerUpEvent += OnPointerUpInToggleButton;
				_buttonEventListener.OnPointerDownEvent += OnPointerDownInToggleButton;
			}
			UpdateToggleColors();
			_initOnNextFrame = false;
		}

		public void Toggle()
		{
			if (_value == 1)
			{
				_value = 0;
			}
			else if (_value == 0)
			{
				_value = 1;
			}
			/*if (OnToggle != null)
			{
				OnToggle(this);
			}*/
			OnOptionValueChanged();
			UpdateToggleColors();
			Locator.GetMenuAudioController().PlayOptionToggle();
		}

		protected virtual void OnEnable()
		{
			UpdateToggleColors();
		}

		protected virtual void OnDisable()
		{
			_uiElementSelected = false;
			_mouseClickInElement = false;
			UpdateToggleColors();
		}

		public virtual bool IsSelected()
		{
			return _uiElementSelected;
		}

		protected virtual void OnPointerUpInToggleButton(PointerEventData eventData, Selectable selectable)
		{
			if (_mouseClickInElement && !eventData.used)
			{
				Toggle();
				if (selectable.gameObject == _toggleElementButton.gameObject && eventData.pointerPress == _toggleElementButton.gameObject)
				{
					selectable.Select();
				}
				UpdateToggleColors();
				eventData.Use();
			}
		}

		protected virtual void OnPointerDownInToggleButton(PointerEventData eventData, Selectable selectable)
		{
			if (selectable.gameObject == _toggleElementButton.gameObject)
			{
				_mouseClickInElement = true;
			}
		}

		protected virtual void OnPointerExitToggleButton(PointerEventData eventData, Selectable selectable)
		{
			UpdateToggleColors();
			_mouseClickInElement = false;
		}

		protected virtual void OnPointerEnterToggleButton(PointerEventData eventData, Selectable selectable)
		{
			if (selectable.gameObject == _toggleElementButton.gameObject)
			{
				_navigationButtonStyleApplier.ChangeState(UIElementState.ROLLOVER_HIGHLIGHT, false);
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			_uiElementSelected = true;
			UpdateToggleColors();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			_uiElementSelected = false;
			UpdateToggleColors();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			eventData.Use();
			Toggle();
			/*if (OnToggleSubmit != null)
			{
				OnToggleSubmit(eventData, this);
			}*/
		}

		public virtual void OnCancel(BaseEventData eventData)
		{
/*			if (OnToggleCancel != null)
			{
				eventData.Use();
				OnToggleCancel(eventData, this);
			}*/
		}

		protected virtual void UpdateToggleColors()
		{
			if (_navigationButtonStyleApplier == null)
			{
				return;
			}
			if (_uiElementSelected)
			{
				_navigationButtonStyleApplier.ChangeState(UIElementState.HIGHLIGHTED, false);
			}
			else
			{
				_navigationButtonStyleApplier.ChangeState(UIElementState.NORMAL, false);
			}
			bool flag = _value == 1;
			if (flag != _toggleGraphic.enabled)
			{
				_toggleGraphic.enabled = flag;
			}
		}
	}
}
