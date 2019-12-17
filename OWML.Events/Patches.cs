using System;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace OWML.Events
{
    internal static class Patches
    {
        public static Action<MonoBehaviour> OnAwake { get; set; }
        public static Action<MonoBehaviour> OnStart { get; set; }

        public static void PostAwake(MonoBehaviour __instance)
        {
            OnAwake?.Invoke(__instance);
        }

        public static void PostStart(MonoBehaviour __instance)
        {
            OnStart?.Invoke(__instance);
        }

        public static IEnumerable<CodeInstruction> EmptyMethod(IEnumerable<CodeInstruction> instructions)
        {
            return new List<CodeInstruction>();
        }

    }
}
