using OWML.Common;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using OWML.Common.Interfaces;

namespace OWML.ModHelper.Input
{
	public class OWMLRebinding : IOWMLRebinding
	{
		public static Dictionary<string, InputActionMap> CustomActionMaps = new();

		public OWMLRebinding(IModConsole console, IHarmonyHelper harmony)
		{
			harmony.AddPrefix(typeof(InputCommandManager).GetMethod("LoadActions", [typeof(string)]), typeof(Patches), nameof(Patches.LoadActions));
		}
	}
}