using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModInputHandler : IModInputHandler
    {
        private class InputInterceptor
        {
            public static void SingleAxisRemovePre(SingleAxisCommand __instance)
            {
                ModInputHandler.Instance.UnregisterGamesBinding(__instance);
            }

            public static void DoubleAxisRemovePre(DoubleAxisCommand __instance)
            {
                ModInputHandler.Instance.UnregisterGamesBinding(__instance);
            }

            public static void SingleAxisUpdatePost(
                SingleAxisCommand __instance, 
                ref float ____value, 
                int ____axisDirection, 
                KeyCode ____gamepadKeyCodePositive, 
                KeyCode ____gamepadKeyCodeNegative, 
                KeyCode ____keyPositive, 
                KeyCode ____keyNegative
            )
            {
                KeyCode positiveKey, negativeKey;
                ModInputHandler.Instance.RegisterGamesBinding(__instance);
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
                if (UnityEngine.Input.GetKey(positiveKey) && ModInputHandler.Instance.ShouldIgnore(positiveKey))
                {
                    ____value -= 1f * axisDirection;
                }
                if (UnityEngine.Input.GetKey(negativeKey) && ModInputHandler.Instance.ShouldIgnore(negativeKey))
                {
                    ____value += 1f * axisDirection;
                }
            }

            public static void DoubleAxisUpdatePost(
                DoubleAxisCommand __instance,
                ref Vector2 ____value, 
                KeyCode ____keyboardXPos, 
                KeyCode ____keyboardYPos, 
                KeyCode ____keyboardXNeg, 
                KeyCode ____keyboardYNeg
            )
            {
                ModInputHandler.Instance.RegisterGamesBinding(__instance);
                if (!OWInput.UsingGamepad())
                {
                    if (UnityEngine.Input.GetKey(____keyboardXPos) && ModInputHandler.Instance.ShouldIgnore(____keyboardXPos))
                    {
                        ____value.x -= 1f;
                    }
                    if (UnityEngine.Input.GetKey(____keyboardXNeg) && ModInputHandler.Instance.ShouldIgnore(____keyboardXNeg))
                    {
                        ____value.x += 1f;
                    }
                    if (UnityEngine.Input.GetKey(____keyboardYPos) && ModInputHandler.Instance.ShouldIgnore(____keyboardYPos))
                    {
                        ____value.y -= 1f;
                    }
                    if (UnityEngine.Input.GetKey(____keyboardYNeg) && ModInputHandler.Instance.ShouldIgnore(____keyboardYNeg))
                    {
                        ____value.y += 1f;
                    }
                }
            }
        }

        internal static ModInputHandler Instance { get; private set; }

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
        private readonly static int _maxComboLength = 7;
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
            Instance = this;
        }

        private void UpdateCombo()
        {
            if (Time.realtimeSinceStartup - _lastUpdate > _updateCooldown)
            {
                _lastUpdate = Time.realtimeSinceStartup;
                Int64 hash = 0;
                int[] keys = new int[_maxComboLength];
                int keysCount = 0;
                bool countdownTrigger = false;
                for (int code = _minUsefulKey; code < _maxUsefulKey; code++)
                {
                    if (Enum.IsDefined(typeof(KeyCode), (KeyCode)code) && UnityEngine.Input.GetKey((KeyCode)code))
                    {
                        keys[keysCount] = code;
                        keysCount++;
                        if (keysCount > _maxComboLength)
                        {
                            hash = (int)RegistrationCode.CombinationTooLong;
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
                        for (int i = 0; i < keysCount; i++)
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
                if (UnityEngine.Input.GetKey(key) && (!ShouldIgnore(key) || _singlePressed.Contains(combination)))
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
            int[] thisCombination = new int[_maxComboLength];
            int i = 0;
            foreach (string key in combo.Split('+'))
            {
                if (i >= _maxComboLength)
                    return (int)RegistrationCode.CombinationTooLong;
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
                        return (int)RegistrationCode.InvalidCombination;
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
                    thisCombination[i] = (int)code;
                }
                else
                {
                    return (int)RegistrationCode.InvalidCombination;
                }
                i++;
            }
            Array.Sort(thisCombination);
            Int64 hash = 0;
            for (i = 0; i < _maxComboLength; i++)
            {
                hash = hash * _maxUsefulKey + thisCombination[i];
            }
            if (hash < _maxUsefulKey && _gameBindingCounter[hash] > 0)
            {
                return (int)RegistrationCode.CombinationTaken;
            }
            return (_comboRegistry.ContainsKey(hash) && !forRemoval ? (int)RegistrationCode.CombinationTaken : hash);
        }

        public RegistrationCode RegisterCombination(IModInputCombination combination)
        {
            var castCombo = (ModInputCombination)combination;
            castCombo.ClearSingles();
            if (castCombo == null || castCombo.Combo == null)
            {
                _console.WriteLine("combination is null");
                return RegistrationCode.InvalidCombination;
            }
            string[] combs = castCombo.Combo.ToLower().Split('/');
            List<Int64> combos = new List<Int64>();
            foreach (string comstr in combs)
            {
                Int64 hash = ParseCombination(comstr);
                if (hash <= 0)
                {
                    return (RegistrationCode)hash;
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
            return RegistrationCode.AllNormal;
        }

        public RegistrationCode UnregisterCombination(IModInputCombination combination)
        {
            var castCombo = (ModInputCombination)combination;
            if (_comboRegistry.ContainsValue(combination))
            {
                if (combination == null || castCombo.Combo == null)
                {
                    _console.WriteLine("combination is null");
                    return RegistrationCode.InvalidCombination;
                }
                string[] individualCombos = castCombo.Combo.ToLower().Split('/');
                List<Int64> hashes = new List<Int64>();
                foreach (string comboString in individualCombos)
                {
                    Int64 hash = ParseCombination(comboString, true);
                    if (hash <= 0)
                    {
                        return (RegistrationCode)hash;
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
                return RegistrationCode.AllNormal;
            }
            else
            {
                return RegistrationCode.InvalidCombination;
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
