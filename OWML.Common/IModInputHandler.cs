using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OWML.Common
{
	public interface IModInputHandler
	{
		int RegisterCombination(IModInputCombination combo);
		int UnregisterCombination(IModInputCombination combo);
		bool IsPressed(IModInputCombination combo);
		bool IsNewlyPressed(IModInputCombination combo, bool keep = false);
		bool WasTapped(IModInputCombination combo);
		bool WasNewlyReleased(IModInputCombination combo, bool keep = false);
	}
}
