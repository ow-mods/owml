using System;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModInputHandler : IModInputHandler
    {
        private const float Cooldown = 0.05f;
        private const float TapDuration = 0.1f;
        private const BindingFlags NonPublic = BindingFlags.NonPublic | BindingFlags.Instance;

        internal static ModInputHandler Instance { get; private set; }

        public IModInputTextures Textures { get; }

        private readonly HashSet<IModInputCombination> _singlesPressed = new HashSet<IModInputCombination>();
        private readonly Dictionary<long, HashSet<IModInputCombination>> _comboRegistry = new Dictionary<long, HashSet<IModInputCombination>>();
        private readonly HashSet<IModInputCombination> _toResetOnNextFrame = new HashSet<IModInputCombination>();
        private readonly float[] _timeout = new float[ModInputLibrary.MaxUsefulKey];
        private readonly int[] _gameBindingCounter = new int[ModInputLibrary.MaxUsefulKey];
        private HashSet<IModInputCombination> _currentCombinations = new HashSet<IModInputCombination>();
        private int _lastSingleUpdate;
        private int _lastCombinationUpdate;
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModInputHandler(IModLogger logger, IModConsole console, IHarmonyHelper patcher, IOwmlConfig owmlConfig, IModEvents events)
        {
            var textures = new ModInputTextures();
            textures.FillTextureLibrary();
            Textures = textures;

            _console = console;
            _logger = logger;

            var listenerObject = new GameObject("GameBindingsChangeListener");
            var listener = listenerObject.AddComponent<BindingChangeListener>();
            listener.Initialize(this, events);

            if (owmlConfig.BlockInput)
            {
                patcher.AddPostfix<SingleAxisCommand>("UpdateInputCommand", typeof(InputInterceptor), nameof(InputInterceptor.SingleAxisUpdatePost));
                patcher.AddPostfix<DoubleAxisCommand>("UpdateInputCommand", typeof(InputInterceptor), nameof(InputInterceptor.DoubleAxisUpdatePost));
            }
            Instance = this;
        }

        internal bool IsPressedAndIgnored(KeyCode key)
        {
            UpdateCurrentCombinations();
            var cleanKey = ModInputLibrary.NormalizeKeyCode(key);
            return UnityEngine.Input.GetKey(cleanKey) &&
                _currentCombinations.Count > 0 &&
                Time.realtimeSinceStartup - _timeout[(int)cleanKey] < Cooldown;
        }

        private long? GetHashFromKeyboard()
        {
            long hash = 0;
            var keysCount = 0;
            var countdownTrigger = true;
            for (var code = ModInputLibrary.MinUsefulKey; code < ModInputLibrary.MaxUsefulKey; code++)
            {
                if (!(Enum.IsDefined(typeof(KeyCode), (KeyCode)code) && UnityEngine.Input.GetKey((KeyCode)code)))
                {
                    continue;
                }
                keysCount++;
                if (keysCount > ModInputLibrary.MaxComboLength)
                {
                    return null;
                }
                hash = hash * ModInputLibrary.MaxUsefulKey + code;
                if (Time.realtimeSinceStartup - _timeout[code] > Cooldown)
                {
                    countdownTrigger = false;
                }
            }
            return countdownTrigger ? -hash : hash;
        }

        private HashSet<IModInputCombination> GetCombinationsFromKeyboard()
        {
            var countdownTrigger = false;
            var nullableHash = GetHashFromKeyboard();
            if (nullableHash == null)
            {
                return new HashSet<IModInputCombination>();
            }
            var hash = (long)nullableHash;
            if (hash < 0)
            {
                countdownTrigger = true;
                hash = -hash;
            }
            if (!_comboRegistry.ContainsKey(hash))
            {
                return new HashSet<IModInputCombination>();
            }

            var combinations = new HashSet<IModInputCombination>(_comboRegistry[hash]);
            if (!_currentCombinations.Equals(combinations) && countdownTrigger)
            {
                combinations.IntersectWith(_currentCombinations);
            }
            if (combinations.Count == 0)
            {
                return combinations;
            }

            while (hash > 0)
            {
                _timeout[hash % ModInputLibrary.MaxUsefulKey] = Time.realtimeSinceStartup;
                hash /= ModInputLibrary.MaxUsefulKey;
            }
            return combinations;
        }

        private void UpdateCurrentCombinations()
        {
            if (_lastCombinationUpdate == Time.frameCount)
            {
                return;
            }
            _lastCombinationUpdate = Time.frameCount;
            foreach (var combo in _toResetOnNextFrame)
            {
                combo.InternalSetPressed(false);
            }
            _toResetOnNextFrame.Clear();
            var combinations = GetCombinationsFromKeyboard();
            if (_currentCombinations.Count > 0 && !_currentCombinations.Equals(combinations))
            {
                var toUnpress = _currentCombinations;
                toUnpress.ExceptWith(combinations);
                toUnpress.ToList().ForEach(combination => combination.InternalSetPressed(false));
                _toResetOnNextFrame.UnionWith(toUnpress);
            }
            _currentCombinations = combinations;
            _currentCombinations.ToList().ForEach(combination => combination.InternalSetPressed());
        }

        public bool IsPressedExact(IModInputCombination combination)
        {
            if (combination == null)
            {
                return false;
            }
            UpdateCurrentCombinations();
            return _currentCombinations.Contains(combination);
        }

        public bool IsNewlyPressedExact(IModInputCombination combination)
        {
            return combination != null &&
                   IsPressedExact(combination) &&
                   combination.IsFirst;
        }

        public bool WasTappedExact(IModInputCombination combination)
        {
            return combination != null &&
                   !IsPressedExact(combination) &&
                   combination.PressDuration < TapDuration &&
                   combination.IsFirst;
        }

        public bool WasNewlyReleasedExact(IModInputCombination combination)
        {
            return combination != null &&
                   !IsPressedExact(combination) &&
                   combination.IsFirst;
        }

        private void CleanupSinglesPressed()
        {
            if (_lastSingleUpdate == Time.frameCount)
            {
                return;
            }
            _lastSingleUpdate = Time.frameCount;
            var toRemove = new List<IModInputCombination>();
            foreach (var combo in _singlesPressed)
            {
                if (!IsPressedSingle(combo))
                {
                    toRemove.Add(combo);
                }
                if (!IsPressed(combo))
                {
                    combo.InternalSetPressed(false);
                    _toResetOnNextFrame.Add(combo);
                }
            }
            _singlesPressed.ExceptWith(toRemove);
        }

        private bool IsPressedSingle(IModInputCombination combination)
        {
            CleanupSinglesPressed();
            if (_currentCombinations.Contains(combination))
            {
                return true;
            }
            var single = combination.Singles.FirstOrDefault(key => UnityEngine.Input.GetKey(key) && !IsPressedAndIgnored(key));
            if (single == 0)
            {
                return false;
            }
            _singlesPressed.Add(combination);
            combination.InternalSetPressed();
            return true;
        }

        public bool IsPressed(IModInputCombination combination)
        {
            return combination != null &&
                   (IsPressedExact(combination) || IsPressedSingle(combination));
        }

        public bool IsNewlyPressed(IModInputCombination combination)
        {
            return combination != null &&
                   IsPressed(combination) &&
                   combination.IsFirst;
        }

        public bool WasTapped(IModInputCombination combination)
        {
            return combination != null &&
                   !IsPressed(combination) &&
                   combination.PressDuration < TapDuration &&
                   combination.IsFirst;
        }

        public bool WasNewlyReleased(IModInputCombination combination)
        {
            return combination != null &&
                   !IsPressed(combination) &&
                   combination.IsFirst;
        }

        private RegistrationCode SwapCombination(IModInputCombination combination, bool toUnregister, bool ignoreTaken)
        {
            var isTaken = false;
            if (combination.Hashes.Count == 0)
            {
                return RegistrationCode.InvalidCombination;
            }
            foreach (var hash in combination.Hashes)
            {
                if (toUnregister)
                {
                    if (!_comboRegistry.ContainsKey(hash))
                    {
                        continue;
                    }
                    _comboRegistry[hash].Remove(combination);
                    if (_comboRegistry[hash].Count == 0)
                    {
                        _comboRegistry.Remove(hash);
                    }
                    continue;
                }
                if (_comboRegistry.ContainsKey(hash) || hash < ModInputLibrary.MaxUsefulKey && _gameBindingCounter[hash] > 0)
                {
                    isTaken = true;
                }
                if (!_comboRegistry.ContainsKey(hash))
                {
                    _comboRegistry.Add(hash, new HashSet<IModInputCombination>());
                }
                if (!isTaken || ignoreTaken)
                {
                    _comboRegistry[hash].Add(combination);
                }
            }
            return isTaken && !ignoreTaken ? RegistrationCode.CombinationTaken : RegistrationCode.AllNormal;
        }

        private List<string> GetCollisions(ReadOnlyCollection<long> hashes)
        {
            var combos = new List<string>();
            foreach (var hash in hashes)
            {
                if (_comboRegistry.ContainsKey(hash))
                {
                    var toAdd = _comboRegistry[hash].Select(combination => combination.FullName).ToList();
                    toAdd.ForEach(combos.Add);
                }
                if (hash < ModInputLibrary.MaxUsefulKey && _gameBindingCounter[hash] > 0) // let's add both ¯\_(ツ)_/¯
                {
                    combos.Add("Outer Wilds." + Enum.GetName(typeof(KeyCode), (KeyCode)hash));
                }
            }
            return combos;
        }

        public List<string> GetWarningMessages(string combinations)
        {
            var hashes = new List<long>();
            var errorMessages = new List<string>();
            foreach (var combo in combinations.Split('/'))
            {
                var hash = ModInputLibrary.StringToHash(combo);
                if (hash <= 0)
                {
                    errorMessages.Add(ModInputLibrary.GetReadableMessage((RegistrationCode)(-hash)));
                    continue;
                }
                hashes.Add(hash);
            }
            var warningMessages = GetCollisions(hashes.AsReadOnly())
                .Select(combination => $"Collides with {combination}").ToList();
            warningMessages.AddRange(errorMessages);
            return warningMessages;
        }

        public IModInputCombination RegisterCombination(IModBehaviour mod, string name, string combination)
        {
            return RegisterCombination(mod, name, combination, false);
        }

        public IModInputCombination RegisterCombination(IModBehaviour mod, string name, string combination, bool ignoreTaken)
        {
            var combo = new ModInputCombination(mod.ModHelper.Manifest, _console, name, combination);
            switch (SwapCombination(combo, false, ignoreTaken))
            {
                case RegistrationCode.InvalidCombination:
                    _console.WriteLine($"Failed to register \"{combo.FullName}\": Invalid combination!", MessageType.Error);
                    return null;
                case RegistrationCode.CombinationTooLong:
                    _console.WriteLine($"Failed to register \"{combo.FullName}\": Too long!", MessageType.Error);
                    return null;
                case RegistrationCode.CombinationTaken:
                    _console.WriteLine($"Failed to register \"{combo.FullName}\": Already in use by following mods:", MessageType.Error);
                    var collisions = GetCollisions(combo.Hashes);
                    foreach (var collision in collisions)
                    {
                        _console.WriteLine($"\"{collision}\"", MessageType.Error);
                    }
                    return null;
                case RegistrationCode.AllNormal:
                    return combo;
                default:
                    return null;
            }
        }

        public void UnregisterCombination(IModInputCombination combination)
        {
            if (combination == null)
            {
                _console.WriteLine("Failed to unregister: Null combination!", MessageType.Error);
                return;
            }
            switch (SwapCombination(combination, true, false))
            {
                case RegistrationCode.InvalidCombination:
                    _console.WriteLine($"Failed to unregister \"{combination.FullName}\": Invalid combination!", MessageType.Error);
                    return;
                case RegistrationCode.CombinationTooLong:
                    _console.WriteLine($"Failed to unregister \"{combination.FullName}\": Too long!", MessageType.Error);
                    return;
                case RegistrationCode.AllNormal:
                    _logger.Log($"Successfully unregistered \"{combination.FullName}\"", MessageType.Success);
                    return;
                default:
                    return;
            }
        }

        internal void RegisterGamesBinding(InputCommand binding)
        {
            if (binding == null)
            {
                return;
            }
            var fields = binding is SingleAxisCommand ?
                typeof(SingleAxisCommand).GetFields(NonPublic) :
                typeof(DoubleAxisCommand).GetFields(NonPublic);
            foreach (var field in fields.Where(x => x.FieldType == typeof(List<KeyCode>)))
            {
                var keys = (List<KeyCode>)field.GetValue(binding);
                foreach (var key in keys.Where(x => x != KeyCode.None))
                {
                    var intKey = (int)ModInputLibrary.NormalizeKeyCode(key);
                    _gameBindingCounter[intKey]++;
                }
            }
        }

        internal void UpdateGamesBindings()
        {
            for (var i = ModInputLibrary.MinUsefulKey; i < ModInputLibrary.MaxUsefulKey; i++)
            {
                _gameBindingCounter[i] = 0;
            }
            var inputCommands = typeof(InputLibrary).GetFields(BindingFlags.Public | BindingFlags.Static).ToList();
            inputCommands.ForEach(field => RegisterGamesBinding(field.GetValue(null) as InputCommand));
        }
    }
}
