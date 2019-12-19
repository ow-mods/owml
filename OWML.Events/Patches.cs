using System;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace OWML.Events
{
    internal static class Patches
    {
        public static Action<MonoBehaviour, Common.Events> OnEvent { get; set; }

        public static void PreAwake(MonoBehaviour __instance)
        {
            OnEvent?.Invoke(__instance, Common.Events.BeforeAwake);
        }

        public static void PreStart(MonoBehaviour __instance)
        {
            OnEvent?.Invoke(__instance, Common.Events.BeforeStart);
        }

        public static void PostAwake(MonoBehaviour __instance)
        {
            OnEvent?.Invoke(__instance, Common.Events.AfterAwake);
        }

        public static void PostStart(MonoBehaviour __instance)
        {
            OnEvent?.Invoke(__instance, Common.Events.AfterStart);
        }

        public static IEnumerable<CodeInstruction> EmptyMethod(IEnumerable<CodeInstruction> instructions)
        {
            return new List<CodeInstruction>();
        }

    }
}
