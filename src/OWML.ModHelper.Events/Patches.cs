﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace OWML.ModHelper.Events
{
	internal static class Patches
	{
		public static event Action<MonoBehaviour, Common.Events> OnEvent;

		public static void BeforeAwake(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.BeforeAwake);

		public static void AfterAwake(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.AfterAwake);

		public static void BeforeStart(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.BeforeStart);

		public static void AfterStart(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.AfterStart);

		public static void BeforeEnable(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.BeforeEnable);

		public static void AfterEnable(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.AfterEnable);

		public static void BeforeDisable(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.BeforeDisable);

		public static void AfterDisable(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.AfterDisable);

		public static void BeforeDestroy(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.BeforeDestroy);

		public static void AfterDestroy(MonoBehaviour __instance) =>
			OnEvent?.Invoke(__instance, Common.Events.AfterDestroy);

		public static bool EmptyMethodPrefix() => false;

		public static IEnumerable<CodeInstruction> EmptyMethod(IEnumerable<CodeInstruction> _) => 
			new List<CodeInstruction>() { new CodeInstruction(OpCodes.Ret) };
	}
}
