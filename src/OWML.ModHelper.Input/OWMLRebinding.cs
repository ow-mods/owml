using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace OWML.ModHelper.Input
{
	public class OWMLRebinding : IOWMLRebinding
	{
		public static InputActionMap CustomActionMap = new("OWMLCustomActionMap");

		public OWMLRebinding(IModConsole console, IHarmonyHelper harmony)
		{
			console.WriteLine($"OWMLREBINDING CONSTRUCTOR");
			harmony.AddPrefix(typeof(InputCommandManager).GetMethod("LoadActions", new Type[] { typeof(string) }), typeof(Patches), nameof(Patches.LoadActions));
		}
	}
}
