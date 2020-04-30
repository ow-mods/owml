using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
	public class ModCombination: IModCombination
	{
		public ModCombination(string combination)
		{
			_combo = combination;
		}
		public string GetCombo()
		{
			return _combo;
		}
		public void _SetPressed(bool state = true)
		{
			if (state)
			{
				if (!_pressed)
				{
					_first = true;
					_firstPressed = Time.realtimeSinceStartup;
				}
				_lastPressed = Time.realtimeSinceStartup;
			}
			else
				_first = true;
			_pressed = state;
		}
		public float GetLastPressedMoment()
		{
			return _lastPressed;
		}
		public float GetPressDuration() { return _lastPressed - _firstPressed; }
		public bool IsFirst(bool keep = false)
		{
			if (_first)
			{
				_first = keep;
				return true;
			}
			return false;
		}
		public void _AddSingle(KeyCode button) { _singles.Add(button); }
		public List<KeyCode> _GetSingles() { return _singles; }
		public void _ClearSingles() { _singles.Clear(); }

		private bool _pressed = false, _first = false;
		private string _combo;
		private float _firstPressed = 0f, _lastPressed = 0f;
		private List<KeyCode> _singles = new List<KeyCode>();
	}
}
