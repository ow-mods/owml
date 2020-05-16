using System;
using System.Reflection;
using System.Collections.ObjectModel;
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
        private const BindingFlags NonPublic = BindingFlags.NonPublic | BindingFlags.Instance;

        internal static ModInputHandler Instance { get; private set; }

        private HashSet<IModInputCombination> _singlesPressed = new HashSet<IModInputCombination>();
        private Dictionary<long, IModInputCombination> _comboRegistry = new Dictionary<long, IModInputCombination>();
        private HashSet<InputCommand> _gameBindingRegistry = new HashSet<InputCommand>();
        private HashSet<IModInputCombination> _toResetOnNextFrame = new HashSet<IModInputCombination>();
        private float[] _timeout = new float[MaxUsefulKey];
        private int[] _gameBindingCounter = new int[MaxUsefulKey];
        private IModInputCombination _currentCombination;
        private int _lastSingleUpdate;
        private int _lastCombinationUpdate;
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
            if (_lastCombinationUpdate != Time.frameCount)
            {
                return;
            }
            _lastCombinationUpdate = Time.frameCount;
            foreach (IModInputCombination combo in _toResetOnNextFrame)
            {
                combo.SetPressed(false);
            }
            _toResetOnNextFrame.Clear();
            var combination = CombinationFromKeyboard();
            if (_currentCombination != null && _currentCombination != combination)
            {
                _currentCombination.SetPressed(false);
                _toResetOnNextFrame.Add(_currentCombination);
            }
            if (combination == null)
            {
                _currentCombination = null;
                return;
            }
            _currentCombination = combination;
            _currentCombination.SetPressed();
        }

        public bool IsPressedExact(IModInputCombination combination)
        {
            UpdateCurrentCombination();
            return _currentCombination == combination;
        }

        public bool IsNewlyPressedExact(IModInputCombination combination)
        {
            UpdateCurrentCombination();
            return _currentCombination == combination && combination.IsFirst;
        }

        public bool WasTappedExact(IModInputCombination combination)
        {
            UpdateCurrentCombination();
            return combination != _currentCombination
                && (combination.PressDuration < TapDuration)
                && combination.IsFirst;
        }

        public bool WasNewlyReleasedExact(IModInputCombination combination)
        {
            UpdateCurrentCombination();
            return _currentCombination != combination && combination.IsFirst;
        }

        private void UpdateSinglesPressed()
        {
            if (_lastSingleUpdate != Time.frameCount)
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
                    combo.SetPressed(false);
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
            foreach (var key in combination.Singles)
            {
                if (UnityEngine.Input.GetKey(key) && !ShouldIgnore(key))
                {
                    _singlesPressed.Add(combination);
                    combination.SetPressed();
                    return true;
                }
            }
            return false;
        }

        public bool IsPressed(IModInputCombination combination)
        {
            return IsPressedExact(combination) || IsPressedSingle(combination);
        }

        public bool IsNewlyPressed(IModInputCombination combination)
        {
            return IsPressed(combination) && combination.IsFirst;
        }

        public bool WasTapped(IModInputCombination combination)
        {
            return (!IsPressed(combination)) && (combination.PressDuration < TapDuration)
                && combination.IsFirst;
        }

        public bool WasNewlyReleased(IModInputCombination combination)
        {
            return (!IsPressed(combination)) && combination.IsFirst;
        }

        private RegistrationCode SwapCombination(IModInputCombination combination, bool toUnregister)
        {
            if (combination.Hashes[0] <= 0)
            {
                return (RegistrationCode)combination.Hashes[0];
            }
            if (!toUnregister)
            {
                foreach (long hash in combination.Hashes)
                {
                    if (_comboRegistry.ContainsKey(hash) || (hash < MaxUsefulKey && _gameBindingCounter[hash] > 0))
                    {
                        return RegistrationCode.CombinationTaken;
                    }
                }
            }
            foreach (long hash in combination.Hashes)
            {
                if (toUnregister)
                {
                    _comboRegistry.Remove(hash);
                    continue;
                }
                _comboRegistry.Add(hash, combination);
            }
            return RegistrationCode.AllNormal;
        }

        private List<string> GetCollisions(ReadOnlyCollection<long> hashes)
        {
            List<string> combos = new List<string>();
            foreach (long hash in hashes)
            {
                if (_comboRegistry.ContainsKey(hash))
                {
                    combos.Add(_comboRegistry[hash].ModName + "." + _comboRegistry[hash].Name);
                }
                if (hash < MaxUsefulKey && _gameBindingCounter[hash] > 0)
                {
                    combos.Add("Outer Wilds." + Enum.GetName(typeof(KeyCode), (KeyCode)hash));
                }
            }
            return combos;
        }

        public IModInputCombination RegisterCombination(IModBehaviour mod, string name, string combination)
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
                var collisions = GetCollisions(combo.Hashes);
                foreach (string collision in collisions)
                {
                    _console.WriteLine(collision);
                }
            }
            _logger.Log($"succesfully registered " + mod.ModHelper.Manifest.Name + "." + name);
            return combo;
        }

        public void UnregisterCombination(IModInputCombination combination)
        {
            var code = SwapCombination(combination, true);
            if (code == RegistrationCode.InvalidCombination)
            {
                _console.WriteLine("Failed to unregister \"" + combination.ModName + "." + combination.Name + "\": invalid combo!");
            }
            else if (code == RegistrationCode.CombinationTooLong)
            {
                _console.WriteLine("Failed to unregister \"" + combination.ModName + "." + combination.Name + "\": too long!");
            }
            _logger.Log($"succesfully unregistered " + combination.ModName + "." + combination.Name);
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
