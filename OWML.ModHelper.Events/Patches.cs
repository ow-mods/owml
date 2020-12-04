using System;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace OWML.ModHelper.Events
{
    internal static class Patches
    {
        public static event Action<MonoBehaviour, Common.Enums.Events> OnEvent;

        public static void BeforeAwake(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.BeforeAwake);
        public static void AfterAwake(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.AfterAwake);

        public static void BeforeStart(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.BeforeStart);
        public static void AfterStart(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.AfterStart);

        public static void BeforeEnable(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.BeforeEnable);
        public static void AfterEnable(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.AfterEnable);

        public static void BeforeDisable(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.BeforeDisable);
        public static void AfterDisable(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.AfterDisable);

        public static void BeforeDestroy(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.BeforeDestroy);
        public static void AfterDestroy(MonoBehaviour __instance) => OnEvent?.Invoke(__instance, Common.Enums.Events.AfterDestroy);

        public static IEnumerable<CodeInstruction> EmptyMethod(IEnumerable<CodeInstruction> instructions)
        {
            return new List<CodeInstruction>();
        }

    }
}
