using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OWML.Common
{
	public enum RegistrationCode
	{
		InvalidCombination = -1,
		CombinationTooLong = -2,
		CombinationTaken = -3,
		AllNormal = 1
	}

	public interface IModInputHandler
	{
		RegistrationCode RegisterCombination(IModInputCombination combo);
		RegistrationCode UnregisterCombination(IModInputCombination combo);
		bool IsPressed(IModInputCombination combo);
		bool IsNewlyPressed(IModInputCombination combo, bool keep = false);
		bool WasTapped(IModInputCombination combo);
		bool WasNewlyReleased(IModInputCombination combo, bool keep = false);
	}
}
