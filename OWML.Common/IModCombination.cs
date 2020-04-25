using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OWML.Common
{
	public interface IModCombination
	{
		string GetCombo();
		void _SetPressed(bool state = true);
		float GetLastPressedMoment();
		float GetPressDuration();
		bool IsFirst(bool keep = false);
		void _AddSingle(KeyCode button);
		List<KeyCode> _GetSingles();
		void _ClearSingles();
	}
}
