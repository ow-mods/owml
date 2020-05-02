using System;
using System.Reflection;
using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModInputHandler : IModInputHandler
    {
        internal static ModInputHandler Instance { get; private set; }

        private HashSet<IModInputCombination> _singlePressed = new HashSet<IModInputCombination>();
        private Dictionary<long, IModInputCombination> _comboRegistry = new Dictionary<long, IModInputCombination>();
        private HashSet<InputCommand> _gameBindingRegistry = new HashSet<InputCommand>();
        private float[] _timeout = new float[_maxUsefulKey];
        private int[] _gameBindingCounter = new int[_maxUsefulKey];
        private ModInputCombination _lastPressed;
        private float _lastUpdate;
        private float _lastSingleUpdate;
        private static readonly float _updateCooldown = 0.01f; //aka little bit less than one frame
        private static readonly float _cooldown = 0.05f;
        private static readonly float _tapKeep = 0.3f;
        private static readonly float _tapDuration = 0.1f;
        private static readonly int _minUsefulKey = 8;
        private static readonly int _maxUsefulKey = 350;
        private static readonly int _maxComboLength = 7;
        private static readonly string xboxPrefix = "xbox_";
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
                long hash = 0;
                var keysCount = 0;
                var countdownTrigger = true;
                var keys = new int[_maxComboLength];
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
                        if (Time.realtimeSinceStartup - _timeout[code] > _cooldown)
                        {
                            countdownTrigger = false;
                        }
                    }
                }
                if (!_comboRegistry.ContainsKey(hash))
                {
                    if (_lastPressed != null)
                    {
                        _lastPressed.SetPressed(false);
                    }
                    _lastPressed = null;
                    return;
                }
                IModInputCombination combination = _comboRegistry[hash];
                if (combination == _lastPressed || !countdownTrigger)
                {
                    if (_lastPressed != null && _lastPressed != combination)
                    {
                        _lastPressed.SetPressed(false);
                    }
                    _lastPressed = (ModInputCombination)combination;
                    _lastPressed.SetPressed();
                    if (keysCount > 1)
                    {
                        for (var i = 0; i < keysCount; i++)
                        {
                            _timeout[keys[i]] = Time.realtimeSinceStartup;
                        }
                    }
                }
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
            return combination != _lastPressed
                && (Time.realtimeSinceStartup - combination.LastPressedMoment < _tapKeep)
                && (combination.PressDuration < _tapDuration);
        }

        private bool IsNewlyReleased_Combo(IModInputCombination combination, bool keep = false)
        {
            UpdateCombo();
            return _lastPressed != combination && combination.IsFirst(keep)
                && (Time.realtimeSinceStartup - combination.LastPressedMoment < _tapKeep);
        }

        private bool IsPressed_Single(IModInputCombination combination)
        {
            if (Time.realtimeSinceStartup - _lastSingleUpdate > _updateCooldown)
            {
                _lastSingleUpdate = Time.realtimeSinceStartup;
                var toRemove = new List<IModInputCombination>();
                foreach (IModInputCombination combo in _singlePressed)
                {
                    if (!IsPressed(combo))
                    {
                        ((ModInputCombination)combo).SetPressed(false);
                        toRemove.Add(combo);
                    }
                }
                foreach (IModInputCombination combo in toRemove)
                {
                    _singlePressed.Remove(combo);
                }
            }
            foreach (var key in ((ModInputCombination)combination).GetSingles())
            {
                if (UnityEngine.Input.GetKey(key) && (!ShouldIgnore(key) || _singlePressed.Contains(combination)))
                {
                    _singlePressed.Add(combination);
                    ((ModInputCombination)combination).SetPressed();
                    return true;
                }
            }
            return false;
        }

        public bool IsPressed(IModInputCombination combination)
        {
            return IsPressed_Single(combination) || IsPressed_Combo(combination);
        }

        public bool IsNewlyPressed(IModInputCombination combination, bool keep = false)
        {
            return IsPressed(combination) && combination.IsFirst(keep);
        }

        public bool WasTapped(IModInputCombination combination)
        {
            return (!(IsPressed_Combo(combination) || IsPressed_Single(combination)))
                && ((ModInputCombination)combination).IsRelevant(_tapKeep)
                && (combination.PressDuration < _tapDuration);
        }

        public bool WasNewlyReleased(IModInputCombination combination, bool keep = false)
        {
            return (!(IsPressed_Combo(combination) || IsPressed_Single(combination)))
                && ((ModInputCombination)combination).IsRelevant(_tapKeep);
        }

        private KeyCode StringToKeyCode(string key)
        {
            key = key.Trim();
            if (key.Contains(xboxPrefix))
            {
                var xboxKey = key.Substring(xboxPrefix.Length);
                var xboxCode = (XboxButton)Enum.Parse(typeof(XboxButton), xboxKey, true);
                if (Enum.IsDefined(typeof(XboxButton), xboxCode))
                {
                    return InputTranslator.GetKeyCode(xboxCode, false);
                }
                return KeyCode.None;
            }
            else
            {
                var changedKey = key;
                if (key == "control" || key == "ctrl")
                {
                    return KeyCode.LeftControl;
                }
                if (key == "shift")
                {
                    return KeyCode.LeftShift;
                }
                if (key == "alt")
                {
                    return KeyCode.LeftAlt;
                }
                var code = (KeyCode)Enum.Parse(typeof(KeyCode), changedKey, true);
                if (Enum.IsDefined(typeof(KeyCode), code))
                {
                    return code;
                }
                return KeyCode.None;
            }
        }

        private long ParseCombination(string combo, bool forRemoval = false)
        {
            var thisCombination = new int[_maxComboLength];
            var i = 0;
            foreach (var key in combo.Trim().ToLower().Split('+'))
            {
                if (i >= _maxComboLength)
                {
                    return (int)RegistrationCode.CombinationTooLong;
                }
                KeyCode code = StringToKeyCode(key);
                if (code == KeyCode.None)
                {
                    return (int)RegistrationCode.InvalidCombination;
                }
                thisCombination[i] = (int)code;
                i++;
            }
            Array.Sort(thisCombination);
            long hash = 0;
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
            var combs = castCombo.Combo.ToLower().Split('/');
            var combos = new List<long>();
            foreach (string comstr in combs)
            {
                var hash = ParseCombination(comstr);
                if (hash <= 0)
                {
                    return (RegistrationCode)hash;
                }
                combos.Add(hash);
            }
            foreach (long comb in combos)
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
            if (!_comboRegistry.ContainsValue(combination))
            {
                return RegistrationCode.InvalidCombination;
            }
            if (combination == null || castCombo.Combo == null)
            {
                _console.WriteLine("combination is null");
                return RegistrationCode.InvalidCombination;
            }
            var hashes = new List<long>();
            foreach (var comboString in castCombo.Combo.ToLower().Split('/'))
            {
                long hash = ParseCombination(comboString, true);
                if (hash <= 0)
                {
                    return (RegistrationCode)hash;
                }
                hashes.Add(hash);
            }
            foreach (var hash in hashes)
            {
                _comboRegistry.Remove(hash);
            }
            _logger.Log("succesfully unregistered " + castCombo.Combo);
            return RegistrationCode.AllNormal;
        }

        internal void RegisterGamesBinding(InputCommand binding)
        {
            if (_gameBindingRegistry.Contains(binding))
            {
                return;
            }
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
                    var key = (KeyCode)(field.GetValue(binding));
                    if (key != KeyCode.None)
                    {
                        _gameBindingCounter[(int)key]++;
                    }
                }
            }
            _gameBindingRegistry.Add(binding);

        }

        internal void UnregisterGamesBinding(InputCommand binding)
        {
            if (!_gameBindingRegistry.Contains(binding))
            {
                return;
            }
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
                    var key = (KeyCode)(field.GetValue(binding));
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
