using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OWML.Common
{
	public interface IModCombination
	{
		float GetLastPressedMoment();
		float GetPressDuration();
		bool IsFirst(bool keep = false);
	}
}
