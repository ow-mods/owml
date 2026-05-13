using OWML.Common;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using OWML.Common.Interfaces;

namespace OWML.ModHelper.Input
{
	public class OWMLRebinding : IOWMLRebinding
	{
		public static Dictionary<string, InputActionMap> CustomActionMaps = new();
		public static List<RebindableID> XAxisRebindables = new();

		public OWMLRebinding(IModConsole console, IHarmonyHelper harmony)
		{
			harmony.AddPrefix(typeof(InputCommandManager).GetMethod(nameof(InputCommandManager.LoadActions), [typeof(string)]), typeof(Patches), nameof(Patches.LoadActions));
			harmony.AddPrefix(typeof(RebindableLookup).GetMethod(nameof(RebindableLookup.IsXAxisRebindable), [typeof(RebindableID)]), typeof(Patches), nameof(Patches.IsXAxisRebindable));
		}
	}
}