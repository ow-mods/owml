using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
	public class ModInputHandler:IModInputHandler
	{
		private class InputInterceptor
		{
			public static void SAxisRemPre(SingleAxisCommand __instance)
			{
				ModInputHandler._self._UnregisterGamesBinding(__instance);
			}
			public static void DAxisRemPre(DoubleAxisCommand __instance)
			{
				ModInputHandler._self._UnregisterGamesBinding(__instance);
			}
			public static void SAxisUpdPost(SingleAxisCommand __instance)
			{
				KeyCode pos, neg;
				ModInputHandler._self._RegisterGamesBinding(__instance);
				FieldInfo val = typeof(SingleAxisCommand).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
				float curval = (float)(val.GetValue(__instance));
				int axisdir = 1;
				if (OWInput.UsingGamepad())
				{
					axisdir = (int)(typeof(SingleAxisCommand).GetField("_axisDirection", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
					pos = (KeyCode)(typeof(SingleAxisCommand).GetField("_gamepadKeyCodePositive", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
					neg = (KeyCode)(typeof(SingleAxisCommand).GetField("_gamepadKeyCodeNegative", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
				}
				else
				{
					pos = (KeyCode)(typeof(SingleAxisCommand).GetField("_keyPositive", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
					neg = (KeyCode)(typeof(SingleAxisCommand).GetField("_keyNegative", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
				}
				if (Input.GetKey(pos) && ModInputHandler._self._ShouldIgnore(pos))
				{
					curval -= 1f * axisdir;
					//DebugInput.logger.Log("succesfully ignored " + pos.ToString());
				}
				if (Input.GetKey(neg) && ModInputHandler._self._ShouldIgnore(neg))
				{
					curval += 1f * axisdir;
					//DebugInput.logger.Log("succesfully ignored " + neg.ToString());
				}
				val.SetValue(__instance, curval);
			}
			public static void DAxisUpdPost(DoubleAxisCommand __instance)
			{
				ModInputHandler._self._RegisterGamesBinding(__instance);
				if (!OWInput.UsingGamepad())
				{
					KeyCode pos, neg;
					FieldInfo val = typeof(DoubleAxisCommand).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
					Vector2 curval = (Vector2)(val.GetValue(__instance));
					pos = (KeyCode)(typeof(DoubleAxisCommand).GetField("_keyboardXPos", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
					neg = (KeyCode)(typeof(DoubleAxisCommand).GetField("_keyboardXNeg", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
					if (Input.GetKey(pos) && ModInputHandler._self._ShouldIgnore(pos))
					{
						curval.x -= 1f;
						//				DebugInput.console.WriteLine("succesfully ignored " + pos.ToString());
					}
					if (Input.GetKey(neg) && ModInputHandler._self._ShouldIgnore(neg))
					{
						curval.x += 1f;
						//				DebugInput.console.WriteLine("succesfully ignored " + neg.ToString());
					}
					pos = (KeyCode)(typeof(DoubleAxisCommand).GetField("_keyboardYPos", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
					neg = (KeyCode)(typeof(DoubleAxisCommand).GetField("_keyboardYNeg", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
					if (Input.GetKey(pos) && ModInputHandler._self._ShouldIgnore(pos))
					{
						curval.y -= 1f;
						//				DebugInput.console.WriteLine("succesfully ignored " + pos.ToString());
					}
					if (Input.GetKey(neg) && ModInputHandler._self._ShouldIgnore(neg))
					{
						curval.y += 1f;
						//				DebugInput.console.WriteLine("succesfully ignored " + neg.ToString());
					}
					val.SetValue(__instance, curval);
				}
			}
		}
		public ModInputHandler(IModLogger logger, IModConsole console, IHarmonyHelper patcher)
		{
			_console = console;
			_logger = logger;
			patcher.AddPrefix<SingleAxisCommand>("ClearBinding", typeof(InputInterceptor), "SAxisRemPre");
			patcher.AddPrefix<DoubleAxisCommand>("ClearBinding", typeof(InputInterceptor), "DAxisRemPre");
			patcher.AddPostfix<SingleAxisCommand>("Update", typeof(InputInterceptor), "SAxisUpdPost");
			patcher.AddPostfix<DoubleAxisCommand>("Update", typeof(InputInterceptor), "DAxisUpdPost");
			_self = this;
		}
		private void UpdateCombo()
		{
			if (Time.realtimeSinceStartup - lastUpdate > 0.01f)
			{
				lastUpdate = Time.realtimeSinceStartup;
				Int64 hash = 0;
				int[] keys = new int[7];
				int t = 0;
				bool countdowntrigger = false;
				for (int code = 8; code < 350; code++)
					if (Enum.IsDefined(typeof(KeyCode), (KeyCode)code) && Input.GetKey((KeyCode)code))
					{
						keys[t] = code;
						t++;
						if (t > 7)
						{
							hash = -2;
							break;
						}
						hash = hash * 350 + code;
						if (Time.realtimeSinceStartup - timeout[code] < cooldown)
							countdowntrigger = true;
					}
				if (comboReg.ContainsKey(hash))
				{
					IModCombination temp = comboReg[hash];
					if (temp == lastPressed || !countdowntrigger)
					{
						lastPressed = comboReg[hash];
						lastPressed._SetPressed();
						for (int i = 0; i < t; i++)
							timeout[keys[i]] = Time.realtimeSinceStartup;
						//DebugInput.console.WriteLine("succesfully recognized combo " + lastPressed.GetCombo());
						return;
					}
				}
				if (lastPressed != null)
					lastPressed._SetPressed(false);
				lastPressed = null;
			}
		}
		private bool IsPressed_Combo(IModCombination comb)
		{
			UpdateCombo();
			return lastPressed == comb;
		}
		private bool IsNewlyPressed_Combo(IModCombination comb, bool keep = false)
		{
			UpdateCombo();
			return lastPressed == comb && comb.IsFirst(keep);
		}
		private bool WasTapped_Combo(IModCombination comb)
		{
			UpdateCombo();
			return comb != lastPressed && (Time.realtimeSinceStartup - comb.GetLastPressedMoment() < tapKeep) && (comb.GetPressDuration() < tapDuration);
		}
		private bool IsNewlyReleased_Combo(IModCombination comb, bool keep = false)
		{
			UpdateCombo();
			return lastPressed != comb && comb.IsFirst(keep) && (Time.realtimeSinceStartup - comb.GetLastPressedMoment() < tapKeep);
		}
		private bool IsPressed_Single(IModCombination comb)
		{
			List<KeyCode> keys = comb._GetSingles();
			foreach (KeyCode key in keys)
				if (Input.GetKey(key) && (!_ShouldIgnore(key) || comb == lastPressed))
				{
					lastPressed = comb;
					timeout[(int)key] = Time.realtimeSinceStartup;
					comb._SetPressed();
					return true;
				}
			return false;
		}
		private bool IsNewlyPressed_Single(IModCombination comb, bool keep = false)
		{
			return IsPressed_Single(comb) && comb.IsFirst(keep);
		}
		private bool WasTapped_Single(IModCombination comb)
		{
			return (!IsPressed_Single(comb)) && (Time.realtimeSinceStartup - comb.GetLastPressedMoment() < tapKeep) && (comb.GetPressDuration() < tapDuration);
		}
		private bool IsNewlyReleased_Single(IModCombination comb, bool keep = false)
		{
			return (!IsPressed_Single(comb)) && comb.IsFirst(keep) && (Time.realtimeSinceStartup - comb.GetLastPressedMoment() < tapKeep);
		}
		public bool IsPressed(IModCombination comb)
		{
			return IsPressed_Combo(comb) || IsPressed_Single(comb);
		}
		public bool IsNewlyPressed(IModCombination comb, bool keep = false)
		{
			return IsPressed(comb) && comb.IsFirst(keep);
		}
		public bool WasTapped(IModCombination comb)
		{
			return WasTapped_Combo(comb) || WasTapped_Single(comb);
		}
		public bool WasNewlyReleased(IModCombination comb, bool keep = false)
		{
			return IsNewlyReleased_Combo(comb, keep) || IsNewlyReleased_Single(comb, keep);
		}
		private Int64 ParseCombo(string combo, bool forRemoval = false)
		{
			combo = combo.Trim().Replace("ctrl", "control");
			Int64[] curcom = new Int64[7];
			int i = 0;
			foreach (string key in combo.Split('+'))
			{
				if (i > 6)
					return -2;
				KeyCode num;
				if (key.Contains("xbox_"))
				{
					string tkey = key.Substring(5);
					XboxButton tnum = (XboxButton)Enum.Parse(typeof(XboxButton), tkey, true);
					if (Enum.IsDefined(typeof(XboxButton), tnum))
						num = InputTranslator.GetKeyCode(tnum, false);
					else
						return -1;
				}
				else
				{
					string tkey = key;
					if (key == "control")
						tkey = "leftcontrol";
					else if (key == "shift")
						tkey = "leftshift";
					else if (key == "alt")
						tkey = "leftalt";
					num = (KeyCode)Enum.Parse(typeof(KeyCode), tkey, true);
				}
				if (Enum.IsDefined(typeof(KeyCode), num))
					curcom[i] = (int)num;
				else
					return -1;
				i++;
			}
			Array.Sort(curcom);
			Int64 hsh = 0;
			for (i = 0; i < 7; i++)
			{
				hsh = hsh * 350 + curcom[i];
			}
			if (hsh < 350)
				if (gamecntr[hsh] > 0)
					return -3;
			return (comboReg.ContainsKey(hsh) && !forRemoval ? -3 : hsh);
		}
		public int RegisterCombo(IModCombination combo)
		{
			combo._ClearSingles();
			if (combo == null || combo.GetCombo() == null)
			{
				_console.WriteLine("combination is null");
				return -1;
			}
			string[] combs = combo.GetCombo().ToLower().Split('/');
			List<Int64> combos = new List<Int64>();
			foreach (string comstr in combs)
			{
				Int64 temp = ParseCombo(comstr);
				if (temp <= 0)
					return (int)temp;
				else
					combos.Add(temp);
			}
			foreach (Int64 comb in combos)
			{
				comboReg.Add(comb, combo);
				if (comb < 350)
					combo._AddSingle((KeyCode)comb);
			}
			_logger.Log("succesfully registered " + combo.GetCombo());
			return 1;
		}
		public int UnregisterCombo(IModCombination combination)
		{
			if (comboReg.ContainsValue(combination))
			{
				if (combination == null || combination.GetCombo() == null)
				{
					_console.WriteLine("combination is null");
					return -1;
				}
				string[] combs = combination.GetCombo().ToLower().Split('/');
				List<Int64> combos = new List<Int64>();
				foreach (string comstr in combs)
				{
					Int64 temp = ParseCombo(comstr, true);
					if (temp <= 0 && temp > -3)
						return (int)temp;
					else
						combos.Add(temp);
				}
				foreach (Int64 comb in combos)
					comboReg.Remove(comb);
				_logger.Log("succesfully unregistered " + combination.GetCombo());
				return -3;
			}
			else
				return 1;
		}

		public void _RegisterGamesBinding(InputCommand binding)
		{
			if (!bindreg.Contains(binding))
			{
				KeyCode key;
				FieldInfo[] fields;
				if (binding is SingleAxisCommand)
					fields = typeof(SingleAxisCommand).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
				else
					fields = typeof(DoubleAxisCommand).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (FieldInfo field in fields)
				{
					if (field.FieldType == typeof(KeyCode))
					{
						key = (KeyCode)(field.GetValue(binding));
						if (key != KeyCode.None)
						{
							gamecntr[(int)key]++;
							//DebugInput.console.WriteLine("Succesfully registered game's binding: " + key.ToString());
						}
					}
				}
				bindreg.Add(binding);
			}
		}
		public void _UnregisterGamesBinding(InputCommand binding)
		{
			if (bindreg.Contains(binding))
			{
				KeyCode key;
				FieldInfo[] fields;
				if (binding is SingleAxisCommand)
					fields = typeof(SingleAxisCommand).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
				else
					fields = typeof(DoubleAxisCommand).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (FieldInfo field in fields)
					if (field.FieldType == typeof(KeyCode))
					{
						key = (KeyCode)(field.GetValue(binding));
						if (key != KeyCode.None)
						{
							gamecntr[(int)key]--;
							//DebugInput.console.WriteLine("Succesfully unregistered game's binding: " + key.ToString());
						}
					}
				bindreg.Remove(binding);
			}
		}
		public bool _ShouldIgnore(KeyCode code)
		{
			UpdateCombo();
			return lastPressed != null && Time.realtimeSinceStartup - timeout[(int)code] < cooldown;
		}

		private float[] timeout = new float[350];
		private int[] gamecntr = new int[350];
		private Dictionary<Int64, IModCombination> comboReg = new Dictionary<Int64, IModCombination>();
		private HashSet<InputCommand> bindreg = new HashSet<InputCommand>();
		private IModCombination lastPressed;
		private float lastUpdate;
		private float cooldown = 0.016f;
		private float tapKeep = 0.3f;
		private float tapDuration = 0.1f;
		private IModLogger _logger;
		private IModConsole _console;
		public static ModInputHandler _self;
	}
}
