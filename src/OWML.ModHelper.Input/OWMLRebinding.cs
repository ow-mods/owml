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

		public static List<IOWMLKeyRebindingElement> ListCustomRebindableOptions = new();

		public OWMLRebinding(IModConsole console, IHarmonyHelper harmony)
		{
			harmony.AddPrefix(typeof(InputCommandManager).GetMethod("LoadActions", new Type[] { typeof(string) }), typeof(Patches), nameof(Patches.LoadActions));

			harmony.AddPrefix(typeof(SettingsMenuView).GetMethod("NotifyBindingChanged"), typeof(Patches), nameof(Patches.NotifyBindingChanged));
			harmony.AddPrefix(typeof(SettingsMenuView).GetMethod("OnButtonImagesChanged", BindingFlags.NonPublic | BindingFlags.Instance), typeof(Patches), nameof(Patches.OnButtonImagesChanged));
			harmony.AddPrefix(typeof(SettingsMenuView).GetMethod("OnSettingsMenuPush", BindingFlags.NonPublic | BindingFlags.Instance), typeof(Patches), nameof(Patches.OnSettingsMenuPush));
			harmony.AddPrefix(typeof(SettingsMenuView).GetMethod("RefreshRebindingDisplay"), typeof(Patches), nameof(Patches.RefreshRebindingDisplay));
			harmony.AddPrefix(typeof(SettingsMenuView).GetMethod("UpdateKeyRebindingElementDisplays"), typeof(Patches), nameof(Patches.UpdateKeyRebindingElementDisplays));

			harmony.AddPrefix(typeof(RebindingState).GetMethod("ApplyBinding", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(InputDevice), typeof(string), typeof(bool)}, null), typeof(Patches), nameof(Patches.ApplyBinding));
		}
	}
}