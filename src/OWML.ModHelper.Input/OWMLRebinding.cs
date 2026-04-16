using OWML.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using OWML.Common.Interfaces.Menus;
using UnityEngine.InputSystem;
using OWML.Common.Interfaces;

namespace OWML.ModHelper.Input
{
	public class OWMLRebinding : IOWMLRebinding
	{
		public static Dictionary<string, InputActionMap> CustomActionMaps = new();

		public OWMLRebinding(IModConsole console, IHarmonyHelper harmony)
		{
			harmony.AddPrefix(typeof(InputCommandManager).GetMethod("LoadActions", new Type[] { typeof(string) }), typeof(Patches), nameof(Patches.LoadActions));

			harmony.AddPrefix(typeof(InputCommandUtils).GetMethod("TryCreateInputCommands"), typeof(Patches), nameof(Patches.TryCreateInputCommands));
		}
	}
}