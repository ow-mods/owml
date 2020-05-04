using System;
using System.Reflection;
using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModInputHandler : IModInputHandler
    {
        private const float Cooldown = 0.05f;
        private const float TapDuration = 0.1f;
        private const int MinUsefulKey = 8;
        private const int MaxUsefulKey = 350;
        private const int MaxComboLength = 7;
        private const string XboxPrefix = "xbox_";
        private const BindingFlags NonPublic = BindingFlags.NonPublic | BindingFlags.Instance;

        internal static ModInputHandler Instance { get; private set; }

        private HashSet<IModInputCombination> _singlePressed = new HashSet<IModInputCombination>();
        private Dictionary<long, IModInputCombination> _comboRegistry = new Dictionary<long, IModInputCombination>();
        private HashSet<InputCommand> _gameBindingRegistry = new HashSet<InputCommand>();
        private float[] _timeout = new float[MaxUsefulKey];
        private int[] _gameBindingCounter = new int[MaxUsefulKey];
        private ModInputCombination _lastPressed;
        private float _lastUpdate;
        private float _lastSingleUpdate;
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        internal bool ShouldIgnore(KeyCode code)
        {
            UpdateCombo();
            return _lastPressed != null && Time.realtimeSinceStartup - _timeout[(int)code] < Cooldown;
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
            if (Time.realtimeSinceStartup - _lastUpdate < Time.unscaledDeltaTime / 2)
            {
                return;
            }
            _lastUpdate = Time.realtimeSinceStartup;
            long hash = 0;
            var keysCount = 0;
            var countdownTrigger = true;
            var keys = new int[MaxComboLength];
            for (int code = MinUsefulKey; code < MaxUsefulKey; code++)
            {
                if (!(Enum.IsDefined(typeof(KeyCode), (KeyCode)code) && UnityEngine.Input.GetKey((KeyCode)code)))
                {
                    continue;
                }
                keys[keysCount] = code;
                keysCount++;
                if (keysCount > MaxComboLength)
                {
                    hash = (int)RegistrationCode.CombinationTooLong;
                    break;
                }
                hash = hash * MaxUsefulKey + code;
                if (Time.realtimeSinceStartup - _timeout[code] > Cooldown)
                {
                    countdownTrigger = false;
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
            var combination = _comboRegistry[hash];
            if (!(combination == _lastPressed) && countdownTrigger)
            {
                return;
            }
            if (_lastPressed != null && _lastPressed != combination)
            {
                _lastPressed.SetPressed(false);
            }
            _lastPressed = (ModInputCombination)combination;
            _lastPressed.SetPressed();
            if (keysCount > 1)
            {
                return;
            }
            for (var i = 0; i < keysCount; i++)
            {
                _timeout[keys[i]] = Time.realtimeSinceStartup;
            }
        }

        public bool IsPressedExact(IModInputCombination combination)
        {
            UpdateCombo();
            return _lastPressed == combination;
        }

        public bool IsNewlyPressedExact(IModInputCombination combination, bool keep = false)
        {
            UpdateCombo();
            return _lastPressed == combination && combination.IsFirst(keep);
        }

        public bool WasTappedExact(IModInputCombination combination, bool keep = false)
        {
            UpdateCombo();
            return combination != _lastPressed
                && (combination.PressDuration < TapDuration)
                && combination.IsFirst(keep);
        }

        public bool WasNewlyReleasedExact(IModInputCombination combination, bool keep = false)
        {
            UpdateCombo();
            return _lastPressed != combination && combination.IsFirst(keep);
        }

        private bool IsPressedSingle(IModInputCombination combination)
        {
            if (Time.realtimeSinceStartup - _lastSingleUpdate > Time.unscaledDeltaTime / 2)
            {
                _lastSingleUpdate = Time.realtimeSinceStartup;
                var toRemove = new List<IModInputCombination>();
                foreach (var combo in _singlePressed)
                {
                    if (!IsPressed(combo))
                    {
                        ((ModInputCombination)combo).SetPressed(false);
                        toRemove.Add(combo);
                    }
                }
                foreach (var combo in toRemove)
                {
                    _singlePressed.Remove(combo);
                }
            }
            foreach (var key in ((ModInputCombination)combination).GetSingles())
            {
                if (UnityEngine.Input.GetKey(key) && !ShouldIgnore(key))
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
            return IsPressedSingle(combination) || IsPressedExact(combination);
        }

        public bool IsNewlyPressed(IModInputCombination combination, bool keep = false)
        {
            return IsPressed(combination) && combination.IsFirst(keep);
        }

        public bool WasTapped(IModInputCombination combination, bool keep = false)
        {
            return (!(IsPressedExact(combination) || IsPressedSingle(combination)))
                && (combination.PressDuration < TapDuration) 
                && combination.IsFirst(keep);
        }

        public bool WasNewlyReleased(IModInputCombination combination, bool keep = false)
        {
            return (!(IsPressedExact(combination) || IsPressedSingle(combination)))
                && combination.IsFirst(keep);
        }

        private KeyCode StringToKeyCode(string key)
        {
            key = key.Trim();
            if (key.Contains(XboxPrefix))
            {
                var xboxKey = key.Substring(XboxPrefix.Length);
                var xboxCode = (XboxButton)Enum.Parse(typeof(XboxButton), xboxKey, true);
                if (Enum.IsDefined(typeof(XboxButton), xboxCode))
                {
                    return InputTranslator.GetKeyCode(xboxCode, false);
                }
                return KeyCode.None;
            }
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

        private long ParseCombination(string combo, bool forRemoval = false)
        {
            var thisCombination = new int[MaxComboLength];
            var i = 0;
            foreach (var key in combo.Trim().ToLower().Split('+'))
            {
                if (i >= MaxComboLength)
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
            for (i = 0; i < MaxComboLength; i++)
            {
                hash = hash * MaxUsefulKey + thisCombination[i];
            }
            if (hash < MaxUsefulKey && _gameBindingCounter[hash] > 0)
            {
                return (int)RegistrationCode.CombinationTaken;
            }
            return (_comboRegistry.ContainsKey(hash) && !forRemoval ? (int)RegistrationCode.CombinationTaken : hash);
        }

        private RegistrationCode SwapCombination(ModInputCombination combination, bool toUnregister)
        {
            if (combination == null || combination.Combo == null)
            {
                _logger.Log("combination is null");
                return RegistrationCode.InvalidCombination;
            }
            if (!_comboRegistry.ContainsValue(combination) && toUnregister)
            {
                return RegistrationCode.InvalidCombination;
            }
            var hashes = new List<long>();
            foreach (var combo in combination.Combo.ToLower().Split('/'))
            {
                var hash = ParseCombination(combo, toUnregister);
                if (hash <= 0)
                {
                    return (RegistrationCode)hash;
                }
                hashes.Add(hash);
            }
            combination.ClearSingles();
            foreach (long hash in hashes)
            {
                if (toUnregister)
                {
                    _comboRegistry.Remove(hash);
                    continue;
                }
                _comboRegistry.Add(hash, combination);
                if (hash < MaxUsefulKey)
                {
                    combination.AddSingle((KeyCode)hash);
                }
            }
            _logger.Log("succesfully registered " + combination.Combo);
            return RegistrationCode.AllNormal;
        }

        public RegistrationCode RegisterCombination(IModInputCombination combination)
        {
            return SwapCombination((ModInputCombination)combination, false);
        }

        public RegistrationCode UnregisterCombination(IModInputCombination combination)
        {
            return SwapCombination((ModInputCombination)combination, true);
        }

        internal void SwapGamesBinding(InputCommand binding, bool toUnregister)
        {
            if (_gameBindingRegistry.Contains(binding) ^ toUnregister)
            {
                return;
            }
            var fields = binding is SingleAxisCommand ?
                typeof(SingleAxisCommand).GetFields(NonPublic) : typeof(DoubleAxisCommand).GetFields(NonPublic);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(KeyCode))
                {
                    var key = (KeyCode)(field.GetValue(binding));
                    if (key != KeyCode.None)
                    {
                        _gameBindingCounter[(int)key] += toUnregister ? -1 : 1;
                    }
                }
            }
            if (toUnregister)
                _gameBindingRegistry.Remove(binding);
            else
                _gameBindingRegistry.Add(binding);
        }

        internal void RegisterGamesBinding(InputCommand binding)
        {
            SwapGamesBinding(binding, false);
        }

        internal void UnregisterGamesBinding(InputCommand binding)
        {
            SwapGamesBinding(binding, true);
        }
    }
}
