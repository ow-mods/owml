using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OWML.Common
{
	public interface IModInputHandler
	{
		int RegisterCombo(IModCombination combo);
		int UnregisterCombo(IModCombination combo);
		void _RegisterGamesBinding(InputCommand binding);
		void _UnregisterGamesBinding(InputCommand binding);
		bool _ShouldIgnore(KeyCode key);
		bool IsPressed(IModCombination combo);
		bool IsNewlyPressed(IModCombination combo, bool keep = false);
		bool WasTapped(IModCombination combo);
		bool WasNewlyReleased(IModCombination combo, bool keep = false);
	}
}
