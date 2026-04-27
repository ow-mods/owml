using HarmonyLib;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.ModHelper.Menus.NewMenuSystem;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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


		/// <summary>
		/// Fixes mouse delta and scroll input handling in BasicInputAction, AxisInputAction, and InputActionPair
		/// </summary>
		/// <remarks>
		/// The original code only supports mouse delta input for the "x" and "y" (not "up", "down", "left", "right"), and didn't support scroll input at all.
		/// </remarks>
		[HarmonyPatch]
		public static class MouseAxisControlPatch
		{
			private static readonly FieldInfo MouseDeltaField =
				AccessTools.Field(typeof(BaseInputManager), nameof(BaseInputManager.MouseDelta));

			private static readonly FieldInfo Vector2XField =
				AccessTools.Field(typeof(Vector2), nameof(Vector2.x));

			private static readonly FieldInfo Vector2YField =
				AccessTools.Field(typeof(Vector2), nameof(Vector2.y));

			private static readonly MethodInfo ResolveMethod =
				AccessTools.Method(typeof(MouseAxisControlPatch), nameof(ResolveMouseAxis));

			[HarmonyTranspiler]
			[HarmonyPatch(typeof(BasicInputAction), nameof(BasicInputAction.DoUpdate))]
			[HarmonyPatch(typeof(AxisInputAction), nameof(AxisInputAction.DoUpdate))]
			[HarmonyPatch(typeof(InputActionPair), nameof(InputActionPair.DoUpdate))]
			private static IEnumerable<CodeInstruction> PatchWithMatcher(
				IEnumerable<CodeInstruction> instructions)
			{
				var matcher = new CodeMatcher(instructions);

				while (true)
				{
					// if (this._mouseAxisControl.name == "x")
					// {
					//	 value = BaseInputManager.MouseDelta.x;
					// }
					// else
					// {
					//	 value = BaseInputManager.MouseDelta.y;
					// }
					matcher.MatchForward(false,
						new CodeMatch(OpCodes.Ldarg_0),
						new CodeMatch(OpCodes.Ldfld),
						new CodeMatch(IsNameGetter),
						new CodeMatch(OpCodes.Ldstr, "x"),
						new CodeMatch(IsStringEquals),
						new CodeMatch(IsBranchFalse),
						new CodeMatch(IsMouseDeltaLoad),
						new CodeMatch(IsVector2AxisField),
						new CodeMatch(IsStoreLocal),
						new CodeMatch(IsBranch),
						new CodeMatch(IsMouseDeltaLoad),
						new CodeMatch(IsVector2AxisField),
						new CodeMatch(IsStoreLocal)
					);

					if (!matcher.IsValid)
						break;

					var labels = matcher.Instruction.labels;
					var field = matcher.InstructionAt(1).operand as FieldInfo;
					var store = matcher.InstructionAt(8).Clone();

					// value = MouseAxisControlPatch.ResolveMouseAxis(this._mouseAxisControl);
					matcher
						.RemoveInstructions(13)
						.Insert(
							new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
							new CodeInstruction(OpCodes.Ldfld, field),
							new CodeInstruction(OpCodes.Call, ResolveMethod),
							store
						);
				}

				return matcher.InstructionEnumeration();
			}

			private static float ResolveMouseAxis(InputControl control)
			{
				if (control == null)
					return 0f;

				var mouse = Mouse.current;
				if (mouse == null)
					return 0f;

				string path = control.path; // e.g. "/Mouse/delta/x"

				Vector2 vector = path.Contains("/scroll")
					? Mouse.current.scroll.ReadValue()
					: BaseInputManager.MouseDelta;

				return path switch
				{
					var p when p.EndsWith("/x") => vector.x,
					var p when p.EndsWith("/y") => vector.y,

					var p when p.EndsWith("/right") => Mathf.Max(vector.x, 0f),
					var p when p.EndsWith("/left") => Mathf.Max(-vector.x, 0f),

					var p when p.EndsWith("/up") => Mathf.Max(vector.y, 0f),
					var p when p.EndsWith("/down") => Mathf.Max(-vector.y, 0f),

					_ => control.ReadValueAsObject() is float value ? value : 0f
				};
			}


			private static bool IsMouseDeltaLoad(CodeInstruction i)
			{
				return (i.opcode == OpCodes.Ldsfld || i.opcode == OpCodes.Ldsflda)
					&& Equals(i.operand, MouseDeltaField);
			}

			private static bool IsVector2AxisField(CodeInstruction i)
			{
				return i.opcode == OpCodes.Ldfld
					&& (Equals(i.operand, Vector2XField) || Equals(i.operand, Vector2YField));
			}

			private static bool IsNameGetter(CodeInstruction i)
			{
				return i.opcode == OpCodes.Callvirt
					&& Equals(i.operand, AccessTools.PropertyGetter(typeof(InputControl), nameof(InputControl.name)));
			}

			private static bool IsStringEquals(CodeInstruction i)
			{
				return i.opcode == OpCodes.Call
					&& Equals(i.operand, AccessTools.Method(typeof(string), "op_Equality"));
			}

			private static bool IsStoreLocal(CodeInstruction i)
			{
				return i.opcode == OpCodes.Stloc_0
					|| i.opcode == OpCodes.Stloc_1
					|| i.opcode == OpCodes.Stloc_2
					|| i.opcode == OpCodes.Stloc_3
					|| i.opcode == OpCodes.Stloc_S;
			}

			private static bool IsBranch(CodeInstruction i)
			{
				return i.opcode == OpCodes.Br || i.opcode == OpCodes.Br_S;
			}

			private static bool IsBranchFalse(CodeInstruction i)
			{
				return i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S;
			}
		}
	}
}
