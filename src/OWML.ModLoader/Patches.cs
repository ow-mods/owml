using HarmonyLib;
using OWML.Common;
using OWML.ModHelper.Menus.NewMenuSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputBinding;

namespace OWML.ModLoader
{
	[HarmonyPatch]
	public static class Patches
	{
		/// <summary>
		/// Modified TryGetAxisIdentifier that removes the device prefix of more than just gamepad.
		/// </summary>
		/// <remarks>
		/// Fixes devices other than gamepad not being able to see button prompt for positive/negative axis inputs
		/// </remarks>
		[HarmonyPatch]
		public static class InputTransitionUtilTryGetAxisIdentifierTwoPathsPatch
		{
			public static MethodBase TargetMethod() => AccessTools.Method(
				typeof(InputTransitionUtil),
				nameof(InputTransitionUtil.TryGetAxisIdentifier),
				new[]
				{
					typeof(string),
					typeof(string),
					typeof(AxisIdentifier).MakeByRefType()
				}
			);

			[HarmonyPrefix]
			public static bool Prefix(
				string firstControlPath,
				string secondControlPath,
				ref AxisIdentifier axisID,
				ref bool __result)
			{
				axisID = AxisIdentifier.NONE;

				if (!TryGetAxisKey(firstControlPath, secondControlPath, out var key))
				{
					__result = false;
					return false;
				}

				__result = TryGetAxisID(key, out axisID);
				return false;
			}

			private static bool TryGetAxisID(string key, out AxisIdentifier axisID)
			{
				return InputTransitionUtil.AxisIDCache.TryGetValue(key, out axisID);
			}

			private static bool TryGetAxisKey(string firstPath, string secondPath, out string key)
			{
				key = null;

				if (string.IsNullOrEmpty(firstPath) || string.IsNullOrEmpty(secondPath))
					return false;

				var first = firstPath.Split(new[] { ControlPathConstants.PATH_SEPARATOR_CHAR }, StringSplitOptions.RemoveEmptyEntries);
				var second = secondPath.Split(new[] { ControlPathConstants.PATH_SEPARATOR_CHAR }, StringSplitOptions.RemoveEmptyEntries);

				if (first.Length == 0 || second.Length == 0)
					return false;

				var axis = GetAxis(first[first.Length - 1], second[second.Length - 1]);
				if (string.IsNullOrEmpty(axis))
					return false;

				var start = IsDevicePrefix(first[0]) ? 1 : 0;
				first[first.Length - 1] = axis;

				key = string.Join(ControlPathConstants.PATH_SEPARATOR, first, start, first.Length - start);
				return true;
			}

			private static string GetAxis(string first, string second)
			{
				if ((first == ControlPathConstants.UP && second == ControlPathConstants.DOWN) || (first == ControlPathConstants.DOWN && second == ControlPathConstants.UP))
					return ControlPathConstants.Y;

				if ((first == ControlPathConstants.LEFT && second == ControlPathConstants.RIGHT) || (first == ControlPathConstants.RIGHT && second == ControlPathConstants.LEFT))
					return ControlPathConstants.X;

				return null;
			}

			private static bool IsDevicePrefix(string part)
			{
				return part == ControlPathConstants.Gamepad.DEVICE || part == ControlPathConstants.Mouse.DEVICE || part == ControlPathConstants.Keyboard.DEVICE;
			}
		}

		[HarmonyPatch]
		public static class InputActionUtilPopulateUITextureListPatch
		{
			public static MethodBase TargetMethod() => AccessTools.Method(
				typeof(InputActionUtil),
				nameof(InputActionUtil.PopulateUITextureList),
				new[]
				{
					typeof(InputAction),
					typeof(List<Texture2D>).MakeByRefType(),
					typeof(bool)
				}
			);

			[HarmonyPrefix]
			public static bool Prefix(
				InputAction action,
				List<Texture2D> textureList,
				bool gamepad,
				ref bool __result)
			{
				if (action == null)
					return true;

				var mask = gamepad
					? InputActionUtil.GamepadBindingMask
					: InputActionUtil.DesktopBindingMask;

				int index = action.GetBindingIndex(mask);
				if (index == -1)
					return true;

				action.GetBindingDisplayString(
					index,
					out var deviceLayoutName,
					out var controlPath,
					DisplayStringOptions.DontUseShortDisplayNames
				);

				// Control paths that aren't attached to an actual control will be empty and return no images, but we don't want that.
				if (!string.IsNullOrEmpty(controlPath))
					return true;

				textureList.Add(ButtonPromptLibrary.s_testButton);

				__result = true;
				return false;
			}
		}
	}
}
