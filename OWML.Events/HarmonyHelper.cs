using System;
using System.Reflection;
using Harmony;
using OWML.Common;
using UnityEngine;

namespace OWML.Events
{
    public class HarmonyHelper : IHarmonyHelper
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public HarmonyHelper(IModLogger logger, IModConsole console)
        {
            _logger = logger;
            _console = console;
        }

        public void AddPrefix<T>(string methodName, Type patchType, string patchMethodName) where T : MonoBehaviour
        {
            var prefix = patchType.GetAnyMethod(patchMethodName);
            if (prefix == null)
            {
                _logger.Log("prefix is null");
                _console.WriteLine("prefix is null");
                return;
            }
            Patch<T>(methodName, prefix, null, null);
        }

        public void AddPostfix<T>(string methodName, Type patchType, string patchMethodName) where T : MonoBehaviour
        {
            var postfix = patchType.GetAnyMethod(patchMethodName);
            if (postfix == null)
            {
                _logger.Log("postfix is null");
                _console.WriteLine("postfix is null");
                return;
            }
            Patch<T>(methodName, null, postfix, null);
        }

        public void EmptyMethod<T>(string methodName) where T : MonoBehaviour
        {
            Transpile<T>(methodName, typeof(Patches), nameof(Patches.EmptyMethod));
        }

        public void Transpile<T>(string methodName, Type patchType, string patchMethodName) where T : MonoBehaviour
        {
            var patchMethod = patchType.GetMethod(patchMethodName);
            if (patchMethod == null)
            {
                _logger.Log("patchMethod is null");
                _console.WriteLine("patchMethod is null");
                return;
            }
            Patch<T>(methodName, null, null, patchMethod);
        }

        public void Patch<T>(string methodName, MethodInfo prefix, MethodInfo postfix, MethodInfo transpiler) where T : MonoBehaviour
        {
            var targetType = typeof(T);
            _logger.Log("Trying to patch " + targetType.Name);
            HarmonyInstance harmony;
            try
            {
                _logger.Log("Creating harmony instance");
                harmony = HarmonyInstance.Create("com.alek.owml");
                _logger.Log("Created harmony instance");
            }
            catch (Exception ex)
            {
                _logger.Log($"Exception while creating harmony instance: {ex}");
                _console.WriteLine($"Exception while creating harmony instance: {ex}");
                return;
            }
            MethodInfo original;
            try
            {
                _logger.Log($"Getting method {methodName} of {targetType.Name}");
                original = targetType.GetAnyMethod(methodName);
                _logger.Log($"Got method {methodName} of {targetType.Name}");
            }
            catch (Exception ex)
            {
                _logger.Log($"Exception while getting method {methodName} of {targetType.Name}: {ex}");
                _console.WriteLine($"Exception while getting method {methodName} of {targetType.Name}: {ex}");
                return;
            }
            if (original == null)
            {
                _logger.Log("original is null");
                _console.WriteLine("original is null");
                return;
            }
            var prefixMethod = prefix == null ? null : new HarmonyMethod(prefix);
            var postfixMethod = postfix == null ? null : new HarmonyMethod(postfix);
            var transpilerMethod = transpiler == null ? null : new HarmonyMethod(transpiler);
            try
            {
                harmony.Patch(original, prefixMethod, postfixMethod, transpilerMethod);
                _logger.Log($"Patched {targetType.Name}!");
            }
            catch (Exception ex)
            {
                _logger.Log($"Exception while patching {targetType.Name}: {ex}");
                _console.WriteLine($"Exception while patching {targetType.Name}: {ex}");
            }
        }

    }
}
