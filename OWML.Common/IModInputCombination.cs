using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OWML.Common
{
	public interface IModInputCombination
	{
		float GetLastPressedMoment();
		float GetPressDuration();
		bool IsFirst(bool keep = false);
	}
}
