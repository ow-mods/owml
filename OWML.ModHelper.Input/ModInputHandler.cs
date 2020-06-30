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

        private readonly HashSet<IModInputCombination> _singlesPressed = new HashSet<IModInputCombination>();
        private readonly Dictionary<long, IModInputCombination> _comboRegistry = new Dictionary<long, IModInputCombination>();
        private readonly HashSet<InputCommand> _gameBindingRegistry = new HashSet<InputCommand>();
        private readonly HashSet<IModInputCombination> _toResetOnNextFrame = new HashSet<IModInputCombination>();
        private readonly float[] _timeout = new float[ModInputLibrary.MaxUsefulKey];
        private readonly int[] _gameBindingCounter = new int[ModInputLibrary.MaxUsefulKey];
        private IModInputCombination _currentCombination;
        private int _lastSingleUpdate;
        private int _lastCombinationUpdate;
        private readonly IModLogger _logger;
        private readonly IModConsole _console;


        public IModInputTextures Textures { get; }

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
            UpdateCurrentCombination();
            var cleanKey = ModInputLibrary.NormalizeKeyCode(key);
            return UnityEngine.Input.GetKey(cleanKey) &&
                _currentCombination != null &&
                Time.realtimeSinceStartup - _timeout[(int)cleanKey] < Cooldown;
        }

        private long? HashFromKeyboard()
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

        private IModInputCombination CombinationFromKeyboard()
        {
            var countdownTrigger = false;
            var nullableHash = HashFromKeyboard();
            if (nullableHash == null)
            {
                return null;
            }
            var hash = (long)nullableHash;
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
            if (combination != _currentCombination && countdownTrigger)
            {
                return null;
            }

            if (hash < ModInputLibrary.MaxUsefulKey)
            {
                return combination;
            }
            while (hash > 0)
            {
                _timeout[hash % ModInputLibrary.MaxUsefulKey] = Time.realtimeSinceStartup;
                hash /= ModInputLibrary.MaxUsefulKey;
            }
            return combination;
        }

        private void UpdateCurrentCombination()
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
            var combination = CombinationFromKeyboard();
            if (_currentCombination != null && _currentCombination != combination)
            {
                _currentCombination.InternalSetPressed(false);
                _toResetOnNextFrame.Add(_currentCombination);
            }
            if (combination == null)
            {
                _currentCombination = null;
                return;
            }
            _currentCombination = combination;
            _currentCombination.InternalSetPressed();
        }

        public bool IsPressedExact(IModInputCombination combination)
        {
            if (combination == null)
            {
                return false;
            }
            UpdateCurrentCombination();
            return _currentCombination == combination;
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

        private void UpdateSinglesPressed()
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
            foreach (var combo in toRemove)
            {
                _singlesPressed.Remove(combo);
            }
        }

        private bool IsPressedSingle(IModInputCombination combination)
        {
            UpdateSinglesPressed();
            if (_currentCombination == combination)
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

        private RegistrationCode SwapCombination(IModInputCombination combination, bool toUnregister)
        {
            var taken = false;
            if (combination.Hashes[0] <= 0)
            {
                return (RegistrationCode)combination.Hashes[0];
            }
            foreach (var hash in combination.Hashes)
            {
                if (toUnregister)
                {
                    _comboRegistry.Remove(hash);
                    continue;
                }
                if (_comboRegistry.ContainsKey(hash) || hash < ModInputLibrary.MaxUsefulKey && _gameBindingCounter[hash] > 0)
                {
                    taken = true;
                    continue;
                }
                _comboRegistry.Add(hash, combination);
            }
            return taken ? RegistrationCode.CombinationTaken : RegistrationCode.AllNormal;
        }

        private List<string> GetCollisions(ReadOnlyCollection<long> hashes)
        {
            var combos = new List<string>();
            foreach (var hash in hashes)
            {
                if (_comboRegistry.ContainsKey(hash))
                {
                    combos.Add(_comboRegistry[hash].FullName);
                }
                else if (hash < ModInputLibrary.MaxUsefulKey && _gameBindingCounter[hash] > 0)
                {
                    combos.Add("Outer Wilds." + Enum.GetName(typeof(KeyCode), (KeyCode)hash));
                }
            }
            return combos;
        }

        public List<string> GetCollisions(string combinations)
        {
            var hashes = new List<long>();
            foreach (var combo in combinations.Split('/'))
            {
                var hash = ModInputLibrary.StringToHash(combo);
                if (hash <= 0)
                {
                    return new List<string> { ((RegistrationCode)(-hash)).ToString() };
                }
                hashes.Add(hash);
            }
            return GetCollisions(hashes.AsReadOnly());
        }

        public IModInputCombination RegisterCombination(IModBehaviour mod, string name, string combination)
        {
            var combo = new ModInputCombination(mod.ModHelper.Manifest, name, combination);
            switch (SwapCombination(combo, false))
            {
                case RegistrationCode.InvalidCombination:
                    _console.WriteLine($"Failed to register \"{combo.FullName}\": invalid combination!");
                    return null;
                case RegistrationCode.CombinationTooLong:
                    _console.WriteLine($"Failed to register \"{combo.FullName}\": too long!");
                    return null;
                case RegistrationCode.CombinationTaken:
                    _console.WriteLine($"Failed to register \"{combo.FullName}\": already in use by following mods:");
                    var collisions = GetCollisions(combo.Hashes);
                    foreach (var collision in collisions)
                    {
                        _console.WriteLine($"\"{collision}\"");
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
                _console.WriteLine("Failed to unregister: null combination!");
                return;
            }
            switch (SwapCombination(combination, true))
            {
                case RegistrationCode.InvalidCombination:
                    _console.WriteLine($"Failed to unregister \"{combination.FullName}\": invalid combination!");
                    return;
                case RegistrationCode.CombinationTooLong:
                    _console.WriteLine($"Failed to unregister \"{combination.FullName}\": too long!");
                    return;
                case RegistrationCode.AllNormal:
                    _logger.Log($"Successfully unregistered \"{combination.FullName}\"");
                    return;
                default:
                    return;
            }
        }

        internal void SwapGamesBinding(InputCommand binding, bool toUnregister)
        {
            if (_gameBindingRegistry.Contains(binding) ^ toUnregister || binding == null)
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
                    _gameBindingCounter[intKey] += toUnregister ? -1 : 1;
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
        
        internal void UpdateGamesBindings()
        {
            _gameBindingRegistry.Clear();
            var inputCommands = typeof(InputLibrary).GetFields(BindingFlags.Public | BindingFlags.Static);
            inputCommands.ToList().ForEach(field => RegisterGamesBinding(field.GetValue(null) as InputCommand));
        }
    }
}