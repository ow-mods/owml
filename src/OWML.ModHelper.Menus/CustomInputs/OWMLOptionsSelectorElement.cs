using OWML.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.CustomInputs
{
	public class OWMLOptionsSelectorElement : OWMLMenuValueOption, IMoveHandler, IEventSystemHandler, ISelectHandler, IDeselectHandler, IPointerExitHandler, IOWMLOptionsSelectorElement
	{
		internal bool _loopAround;
		internal string[] _optionsList;
		internal Text _displayText;
		internal UIStyleApplier _optionsBoxStyleApplier;
		internal Selectable _selectOnLeft;
		internal Selectable _selectOnRight;

		public event OptionValueChangedEvent OnValueChanged;

		protected bool _initOnNextFrame;
		protected bool _amISelected;
		protected bool _callbacksInitialized;

		protected virtual void Update()
		{
			if (_initOnNextFrame)
			{
				Initialize((int)_value, _optionsList);
			}
		}

		protected virtual void OnEnable()
		{
			if (!_callbacksInitialized && _optionsList.Length != 0)
			{
				if (_selectOnLeft != null)
				{
					var button = _selectOnLeft as Button;
					button?.onClick.AddListener(new UnityAction(OnArrowSelectableOnLeftClick));
				}

				if (_selectOnRight != null)
				{
					var button = _selectOnRight as Button;
					button?.onClick.AddListener(new UnityAction(OnArrowSelectableOnRightClick));
				}

				_callbacksInitialized = true;
			}

			UpdateOptionsBoxColors();
		}

		protected virtual void OnDisable()
		{
			_amISelected = false;
			UpdateOptionsBoxColors();
			if (_selectOnLeft != null)
			{
				var button = _selectOnLeft as Button;
				button?.onClick.RemoveListener(new UnityAction(OnArrowSelectableOnLeftClick));
			}

			if (_selectOnRight != null)
			{
				var button = _selectOnRight as Button;
				button?.onClick.RemoveListener(new UnityAction(OnArrowSelectableOnRightClick));
			}

			_callbacksInitialized = false;
		}

		public override void Initialize(int index) => Initialize(index, _optionsList);

		public virtual void Initialize(int index, string[] displayedOptions)
		{
			base.Initialize(index);
			_value = index;
			_optionsList = displayedOptions;
			if (!_initOnNextFrame)
			{
				_initOnNextFrame = true;
				return;
			}

			if (_selectable == null)
			{
				_selectable = this.GetRequiredComponent<Selectable>();
			}

			var component = base.GetComponent<UIStyleApplier>();
			if (component != null && EventSystem.current.currentSelectedGameObject != base.gameObject)
			{
				component.ChangeState(UIElementState.NORMAL, true);
			}

			if (!_callbacksInitialized)
			{
				if (_selectOnLeft != null)
				{
					var button = _selectOnLeft as Button;
					button?.onClick.AddListener(new UnityAction(OnArrowSelectableOnLeftClick));
				}

				if (_selectOnRight != null)
				{
					var button = _selectOnRight as Button;
					button?.onClick.AddListener(new UnityAction(OnArrowSelectableOnRightClick));
				}

				_callbacksInitialized = true;
			}

			SetSelectedOption();
			UpdateOptionsBoxColors();
			UpdateArrowSelectables();
			_initOnNextFrame = false;
			OnValueChanged?.Invoke(_value, _optionsList[_value]);
		}

		public virtual string GetSelectedOption() => _optionsList[_value];

		protected virtual void SetSelectedOption() => _displayText.text = _optionsList[_value];

		protected virtual void UpdateArrowSelectables()
		{
			var flag3 = true;
			var flag4 = true;
			if (!_loopAround)
			{
				if (_value == 0)
				{
					flag3 = false;
				}
				else if (_value == _optionsList.Length - 1)
				{
					flag4 = false;
				}
			}

			if (_selectOnLeft != null)
			{
				_selectOnLeft.interactable = flag3;
			}

			if (_selectOnRight != null)
			{
				_selectOnRight.interactable = flag4;
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			_amISelected = true;
			UpdateOptionsBoxColors();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			_amISelected = false;
			UpdateOptionsBoxColors();
		}

		public virtual void OnPointerExit(PointerEventData pointerEventData) => UpdateOptionsBoxColors();

		protected virtual void UpdateOptionsBoxColors()
		{
			if (_optionsBoxStyleApplier != null)
			{
				if (_amISelected)
				{
					_optionsBoxStyleApplier.ChangeState(UIElementState.HIGHLIGHTED, false);
					return;
				}

				_optionsBoxStyleApplier.ChangeState(UIElementState.INTERMEDIATELY_HIGHLIGHTED, false);
			}
		}

		public virtual void OnMove(AxisEventData eventData)
		{
			if (_optionsList.Length == 0)
			{
				return;
			}

			OptionsMove(eventData.moveVector);
		}

		protected virtual void OptionsMove(Vector2 moveVector)
		{
			var num = _value;
			if (moveVector.x > 0f)
			{
				num++;
			}
			else if (moveVector.x < 0f)
			{
				num--;
			}

			if (num >= _optionsList.Length)
			{
				num = _loopAround ? 0 : _optionsList.Length - 1;
			}
			else if (num < 0)
			{
				num = _loopAround ? _optionsList.Length - 1 : 0;
			}

			if (num != _value)
			{
				_value = num;
				OnValueChanged?.Invoke(_value, _optionsList[_value]);
				SetSelectedOption();
				UpdateArrowSelectables();
				Locator.GetMenuAudioController().PlayOptionToggle();
			}
		}

		public virtual void OnArrowSelectableOnUpClick()
		{
			OptionsMove(new Vector2(0f, 1f));
			Locator.GetMenuInputModule().SelectOnNextUpdate(_selectable);
		}

		public virtual void OnArrowSelectableOnDownClick()
		{
			OptionsMove(new Vector2(0f, -1f));
			Locator.GetMenuInputModule().SelectOnNextUpdate(_selectable);
		}

		public virtual void OnArrowSelectableOnLeftClick()
		{
			OptionsMove(new Vector2(-1f, 0f));
			Locator.GetMenuInputModule().SelectOnNextUpdate(_selectable);
		}

		public virtual void OnArrowSelectableOnRightClick()
		{
			OptionsMove(new Vector2(1f, 0f));
			Locator.GetMenuInputModule().SelectOnNextUpdate(_selectable);
		}
	}
}
