using System;
using System.Reflection;
using Harmony;
using OWML.Common;

namespace OWML.ModHelper.Events
{
    public class HarmonyHelper : IHarmonyHelper
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private readonly IModManifest _manifest;
        private readonly HarmonyInstance _harmony;

        public HarmonyHelper(IModLogger logger, IModConsole console, IModManifest manifest)
        {
            _logger = logger;
            _console = console;
            _manifest = manifest;
            _harmony = CreateInstance();
        }

        private HarmonyInstance CreateInstance()
        {
            HarmonyInstance harmony;
            try
            {
                _logger.Log("Creating harmony instance");
                harmony = HarmonyInstance.Create("com.owml." + _manifest.UniqueName);
                _logger.Log("Created harmony instance");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Exception while creating harmony instance: {ex}");
                return null;
            }
            if (harmony == null)
            {
                _console.WriteLine("Error: harmony instance is null");
            }
            return harmony;
        }

        public void AddPrefix<T>(string methodName, Type patchType, string patchMethodName)
        {
            var prefix = patchType.GetAnyMethod(patchMethodName);
            if (prefix == null)
            {
                _console.WriteLine($"Error in {nameof(AddPrefix)}: {typeof(T).Name}.{methodName} is null");
                return;
            }
            Patch<T>(methodName, prefix, null, null);
        }

        public void AddPostfix<T>(string methodName, Type patchType, string patchMethodName)
        {
            var postfix = patchType.GetAnyMethod(patchMethodName);
            if (postfix == null)
            {
                _console.WriteLine($"Error in {nameof(AddPostfix)}: {typeof(T).Name}.{methodName} is null");
                return;
            }
            Patch<T>(methodName, null, postfix, null);
        }

        public void EmptyMethod<T>(string methodName)
        {
            Transpile<T>(methodName, typeof(Patches), nameof(Patches.EmptyMethod));
        }

        public void Transpile<T>(string methodName, Type patchType, string patchMethodName)
        {
            var patchMethod = patchType.GetAnyMethod(patchMethodName);
            if (patchMethod == null)
            {
                _console.WriteLine($"Error in {nameof(Transpile)}: {typeof(T).Name}.{methodName} is null");
                return;
            }
            Patch<T>(methodName, null, null, patchMethod);
        }

        private void Patch<T>(string methodName, MethodInfo prefix, MethodInfo postfix, MethodInfo transpiler)
        {
            var targetType = typeof(T);
            _logger.Log("Trying to patch " + targetType.Name);
            MethodInfo original;
            try
            {
                _logger.Log($"Getting method {methodName} of {targetType.Name}");
                original = targetType.GetAnyMethod(methodName);
                _logger.Log($"Got method {methodName} of {targetType.Name}");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Exception while getting method {methodName} of {targetType.Name}: {ex}");
                return;
            }
            if (original == null)
            {
                _console.WriteLine($"Error in {nameof(Patch)}: {targetType.Name}.{methodName} is null");
                return;
            }
            var prefixMethod = prefix == null ? null : new HarmonyMethod(prefix);
            var postfixMethod = postfix == null ? null : new HarmonyMethod(postfix);
            var transpilerMethod = transpiler == null ? null : new HarmonyMethod(transpiler);
            try
            {
                _harmony.Patch(original, prefixMethod, postfixMethod, transpilerMethod);
                _logger.Log($"Patched {targetType.Name}!");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Exception while patching {targetType.Name}.{methodName}: {ex}");
            }
        }

    }
}
