using OWML.Common;
using System;
using System.Reflection;
using HarmonyLib;
using HarmonyLib.Tools;
using System.Collections.Generic;
using HarmonyLib.Public.Patching;
using System.Linq;

namespace OWML.ModHelper.Events
{
	public class HarmonyHelper : IHarmonyHelper
	{
		private readonly IModConsole _console;
		private readonly IModManifest _manifest;
		private readonly IOwmlConfig _owmlConfig;
		private readonly Harmony _harmony;

		public HarmonyHelper(IModConsole console, IModManifest manifest, IOwmlConfig owmlConfig)
		{
			_console = console;
			_manifest = manifest;
			_owmlConfig = owmlConfig;
			_harmony = CreateInstance();
		}

		private Harmony CreateInstance()
		{
			Harmony harmony;
			try
			{
				_console.WriteLine($"Creating harmony instance: {_manifest.UniqueName}", MessageType.Debug);
				if (_owmlConfig.DebugMode)
				{
					_console.WriteLine("Enabling Harmony debug mode.", MessageType.Debug);
					FileLog.logPath = $"{_owmlConfig.LogsPath}/Harmony.Log.{DateTime.Now:dd-MM-yyyy-HH.mm.ss}.txt";
					HarmonyFileLog.Enabled = true;
				}
				harmony = new Harmony(_manifest.UniqueName);
			}
			catch (TypeLoadException ex)
			{
				_console.WriteLine($"TypeLoadException ({ex.TypeName}) while creating harmony instance: {ex}", MessageType.Error);
				return null;
			}
			return harmony;
		}

		public void AddPrefix<T>(string methodName, Type patchType, string patchMethodName) =>
			AddPrefix(GetMethod<T>(methodName), patchType, patchMethodName);

		public void AddPrefix(MethodBase original, Type patchType, string patchMethodName)
		{
			var prefix = Utils.TypeExtensions.GetAnyMethod(patchType, patchMethodName);
			if (prefix == null)
			{
				_console.WriteLine($"Error in {nameof(AddPrefix)}: {patchType.Name}.{patchMethodName} is null.", MessageType.Error);
				return;
			}
			Patch(original, prefix, null, null);
		}

		public void AddPostfix<T>(string methodName, Type patchType, string patchMethodName) =>
			AddPostfix(GetMethod<T>(methodName), patchType, patchMethodName);

		public void AddPostfix(MethodBase original, Type patchType, string patchMethodName)
		{
			var postfix = Utils.TypeExtensions.GetAnyMethod(patchType, patchMethodName);
			if (postfix == null)
			{
				_console.WriteLine($"Error in {nameof(AddPostfix)}: {patchType.Name}.{patchMethodName} is null.", MessageType.Error);
				return;
			}
			Patch(original, null, postfix, null);
		}

		public void EmptyMethod<T>(string methodName, bool useTranspile = false) =>
			EmptyMethod(GetMethod<T>(methodName), useTranspile);

		public void EmptyMethod(MethodBase methodInfo, bool useTranspile = false)
		{
			if (useTranspile)
			{
				Transpile(methodInfo, typeof(Patches), nameof(Patches.EmptyMethod));
				return;
			}

			AddPrefix(methodInfo, typeof(Patches), "EmptyMethodPrefix");
		}

		public void Transpile<T>(string methodName, Type patchType, string patchMethodName) =>
			Transpile(GetMethod<T>(methodName), patchType, patchMethodName);

		public void Transpile(MethodBase original, Type patchType, string patchMethodName)
		{
			var patchMethod = Utils.TypeExtensions.GetAnyMethod(patchType, patchMethodName);
			if (patchMethod == null)
			{
				_console.WriteLine($"Error in {nameof(Transpile)}: {patchType.Name}.{patchMethodName} is null.", MessageType.Error);
				return;
			}
			Patch(original, null, null, patchMethod);
		}

		public void Unpatch<T>(string methodName, PatchType patchType = PatchType.All)
		{
			_console.WriteLine($"Unpatching {typeof(T).Name}.{methodName}", MessageType.Debug);

			var method = GetMethod<T>(methodName);
			var patchInfo = method.ToPatchInfo();

			switch (patchType)
			{
				case PatchType.Prefix:
					patchInfo.RemovePrefix(_manifest.UniqueName);
					break;
				case PatchType.Postfix:
					patchInfo.RemovePostfix(_manifest.UniqueName);
					break;
				case PatchType.Transpiler:
					patchInfo.RemoveTranspiler(_manifest.UniqueName);
					break;
				case PatchType.Finalizer:
					patchInfo.RemoveFinalizer(_manifest.UniqueName);
					break;
				case PatchType.ILManipulator:
					patchInfo.RemoveILManipulator(_manifest.UniqueName);
					break;
				case PatchType.ReversePatch:
					throw new NotImplementedException("Cannot unpatch ReversePatch");
				case PatchType.All:
					patchInfo.RemovePostfix(_manifest.UniqueName);
					patchInfo.RemovePrefix(_manifest.UniqueName);
					patchInfo.RemoveTranspiler(_manifest.UniqueName);
					patchInfo.RemoveFinalizer(_manifest.UniqueName);
					patchInfo.RemoveILManipulator(_manifest.UniqueName);
					break;
			}

			var patchFunctions = typeof(PatchInfo).Assembly.GetTypes().First(x => x.Name == "PatchFunctions"); // what the fuck
			var updateWrapperMethod = patchFunctions.GetMethod("UpdateWrapper", BindingFlags.Static | BindingFlags.NonPublic);
			var replacement = (MethodInfo)updateWrapperMethod.Invoke(null, new object[] { method, patchInfo });

			var addReplacementOriginalMethod = typeof(PatchManager).GetMethod("AddReplacementOriginal", BindingFlags.Static | BindingFlags.NonPublic);
			addReplacementOriginalMethod.Invoke(null, new object[] { method, replacement });

			_console.WriteLine($"Unpatched {typeof(T).Name}.{methodName}!", MessageType.Debug);
		}

		private void Patch(MethodBase original, MethodInfo prefix, MethodInfo postfix, MethodInfo transpiler)
		{
			if (original == null)
			{
				_console.WriteLine($"Error in {nameof(Patch)}: original MethodInfo is null.", MessageType.Error);
				return;
			}
			var prefixMethod = prefix == null ? null : new HarmonyMethod(prefix);
			var postfixMethod = postfix == null ? null : new HarmonyMethod(postfix);
			var transpilerMethod = transpiler == null ? null : new HarmonyMethod(transpiler);
			var fullName = $"{original.DeclaringType}.{original.Name}";
			try
			{
				_harmony.Patch(original, prefixMethod, postfixMethod, transpilerMethod);
				_console.WriteLine($"Patched {fullName}!", MessageType.Debug);
			}
			catch (Exception ex)
			{
				_console.WriteLine($"Exception while patching {fullName}: {ex}", MessageType.Error);
			}
		}

		private MethodInfo GetMethod<T>(string methodName)
		{
			var fullName = $"{typeof(T).Name}.{methodName}";
			try
			{
				_console.WriteLine($"Getting method {fullName}", MessageType.Debug);
				var result = Utils.TypeExtensions.GetAnyMethod(typeof(T), methodName);
				if (result == null)
				{
					_console.WriteLine($"Error - method {fullName} not found.", MessageType.Error);
				}
				return result;
			}
			catch (Exception ex)
			{
				_console.WriteLine($"Exception while getting method {fullName}: {ex}", MessageType.Error);
				return null;
			}
		}
	}
}
