using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OWML.Common
{
	public interface IModInputHandler
	{
		int RegisterCombination(IModCombination combo);
		int UnregisterCombination(IModCombination combo);
		bool IsPressed(IModCombination combo);
		bool IsNewlyPressed(IModCombination combo, bool keep = false);
		bool WasTapped(IModCombination combo);
		bool WasNewlyReleased(IModCombination combo, bool keep = false);
	}
}
