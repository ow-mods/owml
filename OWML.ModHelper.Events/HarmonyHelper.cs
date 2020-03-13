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
        private readonly HarmonyInstance _harmony;

        public HarmonyHelper(IModLogger logger, IModConsole console)
        {
            _logger = logger;
            _console = console;
            _harmony = CreateInstance();
        }

        private HarmonyInstance CreateInstance()
        {
            HarmonyInstance harmony;
            try
            {
                _logger.Log("Creating harmony instance");
                harmony = HarmonyInstance.Create("com.alek.owml");
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
            AddPrefix<T>(methodName, patchType, patchMethodName, null);
        }

        public void AddPrefix<T>(string methodName, Type patchType, string patchMethodName, Type[] parameters)
        {
            var harmonyMethod = new HarmonyMethod(patchType, patchMethodName, parameters);
            Patch<T>(methodName, harmonyMethod, null, null);
        }

        public void AddPostfix<T>(string methodName, Type patchType, string patchMethodName)
        {
            AddPostfix<T>(methodName, patchType, patchMethodName, null);
        }

        public void AddPostfix<T>(string methodName, Type patchType, string patchMethodName, Type[] parameters)
        {
            var harmonyMethod = new HarmonyMethod(patchType, patchMethodName, parameters);
            Patch<T>(methodName, null, harmonyMethod, null);
        }

        public void EmptyMethod<T>(string methodName)
        {
            EmptyMethod<T>(methodName, null);
        }

        public void EmptyMethod<T>(string methodName, Type[] parameters)
        {
            Transpile<T>(methodName, typeof(Patches), nameof(Patches.EmptyMethod), parameters);
        }

        public void Transpile<T>(string methodName, Type patchType, string patchMethodName)
        {
            Transpile<T>(methodName, patchType, patchMethodName, null);
        }

        public void Transpile<T>(string methodName, Type patchType, string patchMethodName, Type[] parameters)
        {
            var harmonyMethod = new HarmonyMethod(patchType, patchMethodName, parameters);
            Patch<T>(methodName, null, null, harmonyMethod);
        }

        private void Patch<T>(string methodName, HarmonyMethod prefix, HarmonyMethod postfix, HarmonyMethod transpiler)
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
            try
            {
                _harmony.Patch(original, prefix, postfix, transpiler);
                _logger.Log($"Patched {targetType.Name}!");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Exception while patching {targetType.Name}.{methodName}: {ex}");
            }
        }

    }
}
