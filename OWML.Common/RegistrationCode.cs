using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common
{
	public enum RegistrationCode
	{
		InvalidCombination = -1,
		CombinationTooLong = -2,
		CombinationTaken = -3,
		AllNormal = 1
	}
}
