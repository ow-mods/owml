using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
    public class ModInputHandler : IModInputHandler
    {
        private class InputInterceptor
        {
            public static void SingleAxisRemovePre(SingleAxisCommand __instance)
            {
                ModInputHandler.Self.UnregisterGamesBinding(__instance);
            }

            public static void DoubleAxisRemovePre(DoubleAxisCommand __instance)
            {
                ModInputHandler.Self.UnregisterGamesBinding(__instance);
            }

            public static void SingleAxisUpdatePost(SingleAxisCommand __instance, ref float ____value, int ____axisDirection, KeyCode ____gamepadKeyCodePositive, KeyCode ____gamepadKeyCodeNegative, KeyCode ____keyPositive, KeyCode ____keyNegative)
            {
                KeyCode positiveKey, negativeKey;
                ModInputHandler.Self.RegisterGamesBinding(__instance);
                int axisDirection = 1;
                if (OWInput.UsingGamepad())
                {
                    axisDirection = ____axisDirection;
                    positiveKey = ____gamepadKeyCodePositive;
                    negativeKey = ____gamepadKeyCodeNegative;
                }
                else
                {
                    positiveKey = ____keyPositive;
                    negativeKey = ____keyNegative;
                }
                if (Input.GetKey(positiveKey) && ModInputHandler.Self.ShouldIgnore(positiveKey))
                {
                    ____value -= 1f * axisDirection;
                }
                if (Input.GetKey(negativeKey) && ModInputHandler.Self.ShouldIgnore(negativeKey))
                {
                    ____value += 1f * axisDirection;
                }
            }

            public static void DoubleAxisUpdatePost(DoubleAxisCommand __instance, ref Vector2 ____value, KeyCode ____keyboardXPos, KeyCode ____keyboardYPos, KeyCode ____keyboardXNeg, KeyCode ____keyboardYNeg)
            {
                ModInputHandler.Self.RegisterGamesBinding(__instance);
                if (!OWInput.UsingGamepad())
                {
                    if (Input.GetKey(____keyboardXPos) && ModInputHandler.Self.ShouldIgnore(____keyboardXPos))
                    {
                        ____value.x -= 1f;
                    }
                    if (Input.GetKey(____keyboardXNeg) && ModInputHandler.Self.ShouldIgnore(____keyboardXNeg))
                    {
                        ____value.x += 1f;
                    }
                    if (Input.GetKey(____keyboardYPos) && ModInputHandler.Self.ShouldIgnore(____keyboardYPos))
                    {
                        ____value.y -= 1f;
                    }
                    if (Input.GetKey(____keyboardYNeg) && ModInputHandler.Self.ShouldIgnore(____keyboardYNeg))
                    {
                        ____value.y += 1f;
                    }
                }
            }
        }

        internal static ModInputHandler Self { get; private set; }

        private HashSet<IModInputCombination> _singlePressed = new HashSet<IModInputCombination>();
        private Dictionary<Int64, IModInputCombination> _comboRegistry = new Dictionary<Int64, IModInputCombination>();
        private HashSet<InputCommand> _gameBindingRegistry = new HashSet<InputCommand>();
        private float[] _timeout = new float[_maxUsefulKey];
        private int[] _gameBindingCounter = new int[_maxUsefulKey];
        private ModInputCombination _lastPressed;
        private float _lastUpdate;
        private float _lastSingleUpdate;
        private readonly static float _updateCooldown = 0.01f; //aka little bit less than one frame
        private readonly static float _cooldown = 0.05f;
        private readonly static float _tapKeep = 0.3f;
        private readonly static float _tapDuration = 0.1f;
        private readonly static int _minUsefulKey = 8;
        private readonly static int _maxUsefulKey = 350;
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        internal bool ShouldIgnore(KeyCode code)
        {
            UpdateCombo();
            return _lastPressed != null && Time.realtimeSinceStartup - _timeout[(int)code] < _cooldown;
        }

        public ModInputHandler(IModLogger logger, IModConsole console, IHarmonyHelper patcher)
        {
            _console = console;
            _logger = logger;
            patcher.AddPrefix<SingleAxisCommand>("ClearBinding", typeof(InputInterceptor), nameof(InputInterceptor.SingleAxisRemovePre));
            patcher.AddPrefix<DoubleAxisCommand>("ClearBinding", typeof(InputInterceptor), nameof(InputInterceptor.DoubleAxisRemovePre));
            patcher.AddPostfix<SingleAxisCommand>("Update", typeof(InputInterceptor), nameof(InputInterceptor.SingleAxisUpdatePost));
            patcher.AddPostfix<DoubleAxisCommand>("Update", typeof(InputInterceptor), nameof(InputInterceptor.DoubleAxisUpdatePost));
            Self = this;
        }

        private void UpdateCombo()
        {
            if (Time.realtimeSinceStartup - _lastUpdate > _updateCooldown)
            {
                _lastUpdate = Time.realtimeSinceStartup;
                Int64 hash = 0;
                int[] keys = new int[7];
                int t = 0;
                bool countdownTrigger = false;
                for (int code = _minUsefulKey; code < _maxUsefulKey; code++)
                {
                    if (Enum.IsDefined(typeof(KeyCode), (KeyCode)code) && Input.GetKey((KeyCode)code))
                    {
                        keys[t] = code;
                        t++;
                        if (t > 7)
                        {
                            hash = -2;
                            break;
                        }
                        hash = hash * _maxUsefulKey + code;
                        if (Time.realtimeSinceStartup - _timeout[code] < _cooldown)
                            countdownTrigger = true;
                    }
                }
                if (_comboRegistry.ContainsKey(hash))
                {
                    IModInputCombination combination = _comboRegistry[hash];
                    if (combination == _lastPressed || !countdownTrigger)
                    {
                        if (_lastPressed != null)
                            _lastPressed.SetPressed(false);
                        _lastPressed = (ModInputCombination)combination;
                        _lastPressed.SetPressed();
                        for (int i = 0; i < t; i++)
                            _timeout[keys[i]] = Time.realtimeSinceStartup;
                        return;
                    }
                }
                if (_lastPressed != null)
                {
                    _lastPressed.SetPressed(false);
                }
                _lastPressed = null;
            }
        }

        private bool IsPressed_Combo(IModInputCombination combination)
        {
            UpdateCombo();
            return _lastPressed == combination;
        }

        private bool IsNewlyPressed_Combo(IModInputCombination combination, bool keep = false)
        {
            UpdateCombo();
            return _lastPressed == combination && combination.IsFirst(keep);
        }

        private bool WasTapped_Combo(IModInputCombination combination)
        {
            UpdateCombo();
            return combination != _lastPressed && (Time.realtimeSinceStartup - combination.GetLastPressedMoment() < _tapKeep) && (combination.GetPressDuration() < _tapDuration);
        }

        private bool IsNewlyReleased_Combo(IModInputCombination combination, bool keep = false)
        {
            UpdateCombo();
            return _lastPressed != combination && combination.IsFirst(keep) && (Time.realtimeSinceStartup - combination.GetLastPressedMoment() < _tapKeep);
        }

        private bool IsPressed_Single(IModInputCombination combination)
        {
            if (Time.realtimeSinceStartup - _lastSingleUpdate > _updateCooldown)
            {
                _lastSingleUpdate = Time.realtimeSinceStartup;
                var toRemove = new List<IModInputCombination>();
                foreach (IModInputCombination combo in _singlePressed)
                    if (!IsPressed(combo))
                    {
                        ((ModInputCombination)combo).SetPressed(false);
                        toRemove.Add(combo);
                    }
                foreach (IModInputCombination combo in toRemove)
                    _singlePressed.Remove(combo);
            }
            List<KeyCode> keys = ((ModInputCombination)combination).GetSingles();
            foreach (KeyCode key in keys)
            {
                if (Input.GetKey(key) && (!ShouldIgnore(key) || _singlePressed.Contains(combination)))
                {
                    _singlePressed.Add(combination);
                    _timeout[(int)key] = Time.realtimeSinceStartup;
                    ((ModInputCombination)combination).SetPressed();
                    return true;
                }
            }
            return false;
        }

        private bool IsNewlyPressed_Single(IModInputCombination combination, bool keep = false)
        {
            return IsPressed_Single(combination) && combination.IsFirst(keep);
        }

        private bool WasTapped_Single(IModInputCombination combination)
        {
            return (!IsPressed_Single(combination)) && (Time.realtimeSinceStartup - combination.GetLastPressedMoment() < _tapKeep) && (combination.GetPressDuration() < _tapDuration);
        }

        private bool IsNewlyReleased_Single(IModInputCombination combination, bool keep = false)
        {
            return (!IsPressed_Single(combination)) && combination.IsFirst(keep) && (Time.realtimeSinceStartup - combination.GetLastPressedMoment() < _tapKeep);
        }

        public bool IsPressed(IModInputCombination combination)
        {
            return IsPressed_Combo(combination) || IsPressed_Single(combination);
        }

        public bool IsNewlyPressed(IModInputCombination combination, bool keep = false)
        {
            return IsPressed(combination) && combination.IsFirst(keep);
        }

        public bool WasTapped(IModInputCombination combination)
        {
            return (!(IsPressed_Combo(combination) || IsPressed_Single(combination))) && (Time.realtimeSinceStartup - combination.GetLastPressedMoment() < _tapKeep) && (combination.GetPressDuration() < _tapDuration);
        }

        public bool WasNewlyReleased(IModInputCombination combination, bool keep = false)
        {
            return (!(IsPressed_Combo(combination) || IsPressed_Single(combination))) && (Time.realtimeSinceStartup - combination.GetLastPressedMoment() < _tapKeep);
        }

        private Int64 ParseCombination(string combo, bool forRemoval = false)
        {
            combo = combo.Trim().Replace("ctrl", "control");
            int[] curcom = new int[7];
            int i = 0;
            foreach (string key in combo.Split('+'))
            {
                if (i > 6)
                    return -2;
                KeyCode code;
                if (key.Contains("xbox_"))
                {
                    string xboxKey = key.Substring(5);
                    var xboxCode = (XboxButton)Enum.Parse(typeof(XboxButton), xboxKey, true);
                    if (Enum.IsDefined(typeof(XboxButton), xboxCode))
                    {
                        code = InputTranslator.GetKeyCode(xboxCode, false);
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    string changedKey = key;
                    if (key == "control")
                    {
                        changedKey = "leftcontrol";
                    }
                    else if (key == "shift")
                    {
                        changedKey = "leftshift";
                    }
                    else if (key == "alt")
                    {
                        changedKey = "leftalt";
                    }
                    code = (KeyCode)Enum.Parse(typeof(KeyCode), changedKey, true);
                }
                if (Enum.IsDefined(typeof(KeyCode), code))
                {
                    curcom[i] = (int)code;
                }
                else
                {
                    return -1;
                }
                i++;
            }
            Array.Sort(curcom);
            Int64 hash = 0;
            for (i = 0; i < 7; i++)
            {
                hash = hash * _maxUsefulKey + curcom[i];
            }
            if (hash < _maxUsefulKey && _gameBindingCounter[hash] > 0)
            {
                return -3;
            }
            return (_comboRegistry.ContainsKey(hash) && !forRemoval ? -3 : hash);
        }

        public int RegisterCombination(IModInputCombination combination)
        {
            var castCombo = (ModInputCombination)combination;
            castCombo.ClearSingles();
            if (castCombo == null || castCombo.Combo == null)
            {
                _console.WriteLine("combination is null");
                return -1;
            }
            string[] combs = castCombo.Combo.ToLower().Split('/');
            List<Int64> combos = new List<Int64>();
            foreach (string comstr in combs)
            {
                Int64 hash = ParseCombination(comstr);
                if (hash <= 0)
                {
                    return (int)hash;
                }
                else
                {
                    combos.Add(hash);
                }
            }
            foreach (Int64 comb in combos)
            {
                _comboRegistry.Add(comb, combination);
                if (comb < _maxUsefulKey)
                {
                    castCombo.AddSingle((KeyCode)comb);
                }
            }
            _logger.Log("succesfully registered " + castCombo.Combo);
            return 1;
        }

        public int UnregisterCombination(IModInputCombination combination)
        {
            var castCombo = (ModInputCombination)combination;
            if (_comboRegistry.ContainsValue(combination))
            {
                if (combination == null || castCombo.Combo == null)
                {
                    _console.WriteLine("combination is null");
                    return -1;
                }
                string[] individualCombos = castCombo.Combo.ToLower().Split('/');
                List<Int64> hashes = new List<Int64>();
                foreach (string comboString in individualCombos)
                {
                    Int64 hash = ParseCombination(comboString, true);
                    if (hash <= 0 && hash > -3)
                    {
                        return (int)hash;
                    }
                    else
                    {
                        hashes.Add(hash);
                    }
                }
                foreach (Int64 comb in hashes)
                {
                    _comboRegistry.Remove(comb);
                }
                _logger.Log("succesfully unregistered " + castCombo.Combo);
                return -3;
            }
            else
            {
                return 1;
            }
        }

        internal void RegisterGamesBinding(InputCommand binding)
        {
            if (!_gameBindingRegistry.Contains(binding))
            {
                KeyCode key;
                FieldInfo[] fields;
                if (binding is SingleAxisCommand)
                {
                    fields = typeof(SingleAxisCommand).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                }
                else
                {
                    fields = typeof(DoubleAxisCommand).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                }
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(KeyCode))
                    {
                        key = (KeyCode)(field.GetValue(binding));
                        if (key != KeyCode.None)
                        {
                            _gameBindingCounter[(int)key]++;
                        }
                    }
                }
                _gameBindingRegistry.Add(binding);
            }
        }

        internal void UnregisterGamesBinding(InputCommand binding)
        {
            if (_gameBindingRegistry.Contains(binding))
            {
                KeyCode key;
                FieldInfo[] fields;
                if (binding is SingleAxisCommand)
                {
                    fields = typeof(SingleAxisCommand).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                }
                else
                {
                    fields = typeof(DoubleAxisCommand).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                }
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(KeyCode))
                    {
                        key = (KeyCode)(field.GetValue(binding));
                        if (key != KeyCode.None)
                        {
                            _gameBindingCounter[(int)key]--;
                        }
                    }
                }
                _gameBindingRegistry.Remove(binding);
            }
        }
    }
}
