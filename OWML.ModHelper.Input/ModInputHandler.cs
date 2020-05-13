using System;
using System.Reflection;
using System.Collections.Generic;
using OWML.Common;
using UnityEngine;
using System.Security.Policy;

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

        private HashSet<IModInputCombination> _singlesPressed = new HashSet<IModInputCombination>();
        private Dictionary<long, ModInputCombination> _comboRegistry = new Dictionary<long, ModInputCombination>();
        private HashSet<InputCommand> _gameBindingRegistry = new HashSet<InputCommand>();
        private float[] _timeout = new float[MaxUsefulKey];
        private int[] _gameBindingCounter = new int[MaxUsefulKey];
        private ModInputCombination _currentCombination;
        private float _lastCombinationUpdate;
        private float _lastSingleUpdate;
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        internal bool ShouldIgnore(KeyCode code)
        {
            UpdateCurrentCombination();
            return _currentCombination != null && Time.realtimeSinceStartup - _timeout[(int)code] < Cooldown;
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

        private long HashFromKeyboard()
        {
            long hash = 0;
            var keysCount = 0;
            var keys = new int[MaxComboLength];
            var countdownTrigger = true;
            for (var code = MinUsefulKey; code < MaxUsefulKey; code++)
            {
                if (!(Enum.IsDefined(typeof(KeyCode), (KeyCode)code) && UnityEngine.Input.GetKey((KeyCode)code)))
                {
                    continue;
                }
                keys[keysCount] = code;
                keysCount++;
                if (keysCount > MaxComboLength)
                {
                    return (int)RegistrationCode.CombinationTooLong;
                }
                hash = hash * MaxUsefulKey + code;
                if (Time.realtimeSinceStartup - _timeout[code] > Cooldown)
                {
                    countdownTrigger = false;
                }
            }
            return countdownTrigger ? -hash : hash; 
        }

        private IModInputCombination CombinationFromKeyboard()
        {
            var countdownTrigger = false;
            var hash = HashFromKeyboard();
            if (hash < 0)
            {
                countdownTrigger = true;
                hash = -hash;
            }
            if (!_comboRegistry.ContainsKey(hash))
            {
                return null;
            }

            var combination = _comboRegistry[hash];
            if (!(combination == _currentCombination) && countdownTrigger)
            {
                return null;
            }

            if (hash < MaxUsefulKey)
            {
                return combination;
            }
            while (hash > 0)
            {
                _timeout[hash % MaxUsefulKey] = Time.realtimeSinceStartup;
                hash /= MaxUsefulKey;
            }
            return combination;
        }

        private void UpdateCurrentCombination()
        {
            if (Time.realtimeSinceStartup - _lastCombinationUpdate < Time.unscaledDeltaTime / 2)
            {
                return;
            }
            _lastCombinationUpdate = Time.realtimeSinceStartup;
            var combination = CombinationFromKeyboard();
            if (_currentCombination != null && _currentCombination != combination)
            {
                _currentCombination.SetPressed(false);
            }
            if (combination == null)
            {
                _currentCombination = null;
                return;
            }
            _currentCombination = (ModInputCombination)combination;
            _currentCombination.SetPressed();
        }

        public bool IsPressedExact(IModInputCombination combination)
        {
            UpdateCurrentCombination();
            return _currentCombination == combination;
        }

        public bool IsNewlyPressedExact(IModInputCombination combination, bool keep = false)
        {
            UpdateCurrentCombination();
            return _currentCombination == combination && combination.IsFirst(keep);
        }

        public bool WasTappedExact(IModInputCombination combination, bool keep = false)
        {
            UpdateCurrentCombination();
            return combination != _currentCombination
                && (combination.PressDuration < TapDuration)
                && combination.IsFirst(keep);
        }

        public bool WasNewlyReleasedExact(IModInputCombination combination, bool keep = false)
        {
            UpdateCurrentCombination();
            return _currentCombination != combination && combination.IsFirst(keep);
        }

        private void UpdateSinglesPressed()
        {
            if (!(Time.realtimeSinceStartup - _lastSingleUpdate > Time.unscaledDeltaTime / 2))
            {
                return;
            }
            _lastSingleUpdate = Time.realtimeSinceStartup;
            var toRemove = new List<IModInputCombination>();
            foreach (var combo in _singlesPressed)
            {
                if (!IsPressed(combo))
                {
                    ((ModInputCombination)combo).SetPressed(false);
                    toRemove.Add(combo);
                }
            }
            foreach (var combo in toRemove)
            {
                _singlesPressed.Remove(combo);
            }
        }

        private bool IsPressedSingle(IModInputCombination combination)
        {
            foreach (var key in ((ModInputCombination)combination).GetSingles())
            {
                if (UnityEngine.Input.GetKey(key) && !ShouldIgnore(key))
                {
                    _singlesPressed.Add(combination);
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

        private KeyCode StringToKeyCodeKeyboard(string keyboardKey)
        {
            if (keyboardKey == "control" || keyboardKey == "ctrl")
            {
                return KeyCode.LeftControl;
            }
            if (keyboardKey == "shift")
            {
                return KeyCode.LeftShift;
            }
            if (keyboardKey == "alt")
            {
                return KeyCode.LeftAlt;
            }
            var code = (KeyCode)Enum.Parse(typeof(KeyCode), keyboardKey, true);
            return Enum.IsDefined(typeof(KeyCode), code) ? code : KeyCode.None;
        }

        private KeyCode StringToKeyCodeXbox(string xboxKey)
        {
            var xboxCode = (XboxButton)Enum.Parse(typeof(XboxButton), xboxKey, true);
            return (Enum.IsDefined(typeof(XboxButton), xboxCode)) ?
                InputTranslator.GetKeyCode(xboxCode, false) : KeyCode.None;
        }

        private KeyCode StringToKeyCode(string key)
        {
            key = key.Trim();
            if (!key.Contains(XboxPrefix))
            {
                return StringToKeyCodeKeyboard(key);
            }
            return StringToKeyCodeXbox(key.Substring(XboxPrefix.Length));
        }

        private int[] StringToKeyArray(string stringCombination)
        {

            var keyCombination = new int[MaxComboLength];
            var i = 0;
            foreach (var key in stringCombination.Trim().ToLower().Split('+'))
            {
                KeyCode code = StringToKeyCode(key);
                if (code == KeyCode.None)
                {
                    keyCombination[0] = (int)RegistrationCode.InvalidCombination;
                    return keyCombination;
                }
                if (i >= MaxComboLength)
                {
                    keyCombination[0] = (int)RegistrationCode.CombinationTooLong;
                    return keyCombination;
                }
                keyCombination[i] = (int)code;
                i++;
            }
            Array.Sort(keyCombination);
            return keyCombination;
        }

        private long StringToHash(string stringCombination, bool forRemoval = false)
        {
            var keyCombination = StringToKeyArray(stringCombination);
            if (keyCombination[0] < 0)
            {
                return keyCombination[0];
            }
            long hash = 0;
            for (var i = 0; i < MaxComboLength; i++)
            {
                hash = hash * MaxUsefulKey + keyCombination[i];
            }
            if (hash < MaxUsefulKey && _gameBindingCounter[hash] > 0 && !forRemoval)
            {
                return (int)RegistrationCode.CombinationTaken;
            }
            return (_comboRegistry.ContainsKey(hash) && !forRemoval ? (int)RegistrationCode.CombinationTaken : hash);
        }

        private List<long> StringToHashes(string combinations, bool forRemoval = false)
        {
            var hashes = new List<long>();
            foreach (var combo in combinations.Split('/'))
            {
                var hash = StringToHash(combo, forRemoval);
                hashes.Add(hash);
                if (hash <= 0)
                {
                    return hashes;
                }
            }
            return hashes;
        }

        private RegistrationCode SwapCombination(ModInputCombination combination, bool toUnregister)
        {
            if ((combination?.Combo == null) || (!_comboRegistry.ContainsValue(combination) && toUnregister))
            {
                return RegistrationCode.InvalidCombination;
            }
            var hashes = StringToHashes(combination.Combo.ToLower(), toUnregister);
            if (hashes[hashes.Count - 1] <= 0)
            {
                return (RegistrationCode)hashes[hashes.Count - 1];
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
            _logger.Log($"succesfully {(toUnregister ? "un" : "")}registered " + combination.Combo);
            return RegistrationCode.AllNormal;
        }

        private List<ModInputCombination> GetCollisions(string combination)
        {
            List<ModInputCombination> combos = new List<ModInputCombination>();
            var hashes = StringToHashes(combination.ToLower(), true);
            foreach (long hash in hashes)
            {
                if (_comboRegistry.ContainsKey(hash))
                {
                    combos.Add(_comboRegistry[hash]);
                }
            }
            return combos;
        }

        public ModInputCombination RegisterCombination(IModBehaviour mod, string name, string combination)
        {
            var combo = new ModInputCombination(mod.ModHelper.Manifest, name, combination);
            var code = SwapCombination(combo, false);
            if (code == RegistrationCode.InvalidCombination)
            {
                _console.WriteLine("Failed to register \"" + mod.ModHelper.Manifest.Name + "." + name + "\": invalid combo!");
            }
            else if (code == RegistrationCode.CombinationTooLong)
            {
                _console.WriteLine("Failed to register \"" + mod.ModHelper.Manifest.Name + "." + name + "\": too long!");
            }
            else if (code == RegistrationCode.CombinationTaken)
            {
                _console.WriteLine("Failed to register \"" + mod.ModHelper.Manifest.Name + "." + name + "\": already in use by following mods:");
                var collisions = GetCollisions(combination);
                foreach (ModInputCombination collision in collisions)
                {
                    _console.WriteLine(collision.ModName + "." + collision.Name);
                }
            }
            return combo;
        }

        public void UnregisterCombination(IModInputCombination combination)
        {
            var castComboination = (ModInputCombination)combination;
            var code = SwapCombination(castComboination, true);
            if (code == RegistrationCode.InvalidCombination)
            {
                _console.WriteLine("Failed to unregister \"" + castComboination.ModName + "." + castComboination.Name + "\": invalid combo!");
            }
            else if (code == RegistrationCode.CombinationTooLong)
            {
                _console.WriteLine("Failed to unregister \"" + castComboination.ModName + "." + castComboination.Name + "\": too long!");
            }
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
            {
                _gameBindingRegistry.Remove(binding);
            }
            else
            {
                _gameBindingRegistry.Add(binding);
            }
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
