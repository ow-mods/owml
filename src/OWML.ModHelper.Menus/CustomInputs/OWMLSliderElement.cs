using Mono.Cecil;
using OWML.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace OWML.ModHelper.Menus.CustomInputs
{
	internal class OWMLSliderElement : OWMLMenuValueOption, IMoveHandler, IEventSystemHandler, IOWMLSliderElement
	{
		private Slider _slider;
		private Selectable _rootSelectable;
		private int _cachedValue;
		private bool _initOnNextFrame;
		private bool _listenerAttached;

		private float _lower = 0;
		private float _upper = 10;

		public event FloatOptionValueChangedEvent OnValueChanged;

		public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
			=> ((value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget)) + fromTarget;

		public virtual void Initialize(float startingValue, float lower, float upper)
		{
			Initialize(Mathf.RoundToInt(Map(startingValue, lower, upper, 0f, 10f)));
			_lower = lower;
			_upper = upper;
		}

		public override void Initialize(int startingValue)
		{
			base.Initialize(startingValue);
			_cachedValue = startingValue;
			if (!_initOnNextFrame)
			{
				_initOnNextFrame = true;
				return;
			}
			_slider = this.GetRequiredComponentInChildren<Slider>();
			_slider.minValue = 0f;
			_slider.maxValue = 10f;
			_slider.wholeNumbers = true;
			_slider.value = _cachedValue;
			if (!_listenerAttached)
			{
				_slider.onValueChanged.AddListener(delegate
				{
					OnSliderValueChanged();
				});
				_listenerAttached = true;
			}

			_rootSelectable = this.GetRequiredComponent<Selectable>();
			var navigation = _rootSelectable.navigation;
			_slider.navigation = navigation;
			_initOnNextFrame = false;
		}

		private void Update()
		{
			if (_initOnNextFrame)
			{
				Initialize(_cachedValue);
			}
		}

		public float GetFloatValue()
		{
			if (_initOnNextFrame)
			{
				Initialize(_cachedValue);
			}

			return Map(_slider.value, 0, 10, _lower, _upper);
		}

		public void OnMove(AxisEventData eventData)
		{
			if (_slider == null)
			{
				return;
			}

			var navigation = _rootSelectable.navigation;
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

			_slider.OnMove(eventData);
		}

		private void OnSliderValueChanged()
		{
			Locator.GetMenuAudioController().PlaySliderIncrement();
			_value = (int)_slider.value;
			OnValueChanged?.Invoke(GetFloatValue());
			Locator.GetMenuInputModule().SelectOnNextUpdate(_rootSelectable);
		}
	}
}
