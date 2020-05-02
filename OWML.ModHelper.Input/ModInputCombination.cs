using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
	public class ModInputCombination : IModInputCombination
	{
		private bool _pressed = false, _first = false;
		private float _firstPressed = 0f, _lastPressed = 0f;
		private List<KeyCode> _singles = new List<KeyCode>();
		internal string Combo { get; private set; }

		public ModInputCombination(string combination)
		{
			Combo = combination;
		}

		public float GetLastPressedMoment() { return _lastPressed; }

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

		internal void SetPressed(bool state = true)
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
			{
				_first = true;
			}
			_pressed = state;
		}

		internal void AddSingle(KeyCode button) { _singles.Add(button); }

		internal List<KeyCode> GetSingles() { return _singles; }

		internal void ClearSingles() { _singles.Clear(); }
	}
}
