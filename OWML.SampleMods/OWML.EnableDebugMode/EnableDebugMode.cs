using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using UnityEngine;
using System.Collections.Generic;

namespace OWML.EnableDebugMode
{
    public class EnableDebugMode : ModBehaviour
    {
        private int _renderValue;
        private bool _isStarted;
        private PlayerSpawner _playerSpawner;
        private Dictionary<string, IModInputCombination> _inputs;

        public override void Configure(IModConfig config)
        {
            if (_inputs != null)
            {
                foreach (var key in _inputs.Keys)
                {
                    ModHelper.Input.UnregisterCombination(_inputs[key]);
                }
            }
            _inputs = new Dictionary<string, IModInputCombination>();
            foreach (var key in config.Settings.Keys)
            {
                var value = config.GetSettingsValue<string>(key);
                if (!string.IsNullOrEmpty(value))
                {
                    var combination = ModHelper.Input.RegisterCombination(this, key, value);
                    _inputs.Add(key, combination);
                }
            }
        }

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(EnableDebugMode)}!", MessageType.Info);
            ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
            ModHelper.Events.Subscribe<PlayerSpawner>(Events.AfterAwake);
            ModHelper.Events.OnEvent += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            if (behaviour is PlayerSpawner playerSpawner && ev == Events.AfterAwake)
            {
                _playerSpawner = playerSpawner;
                _isStarted = true;
            }
        }

        private void Update()
        {
            if (!_isStarted)
            {
                return;
            }

            if (ModHelper.Input.IsNewlyPressed(_inputs["Cycle GUI mode"]))
            {
                CycleGUIMode();
            }

            if (ModHelper.Input.IsNewlyPressed(_inputs["Warp to Interloper"]))
            {
                WarpTo(SpawnLocation.Comet);
            }
            if (ModHelper.Input.IsNewlyPressed(_inputs["Warp to Twins"]))
            {
                WarpTo(SpawnLocation.HourglassTwin_1);
            }
            if (ModHelper.Input.IsNewlyPressed(_inputs["Warp to Timber Hearth"]))
            {
                WarpTo(SpawnLocation.TimberHearth);
            }
            if (ModHelper.Input.IsNewlyPressed(_inputs["Warp to Brittle Hollows"]))
            {
                WarpTo(SpawnLocation.BrittleHollow);
            }
            if (ModHelper.Input.IsNewlyPressed(_inputs["Warp to Giants Deep"]))
            {
                WarpTo(SpawnLocation.GasGiant);
            }
            if (ModHelper.Input.IsNewlyPressed(_inputs["Warp to Dark Bramble"]))
            {
                WarpTo(SpawnLocation.DarkBramble);
            }
            if (ModHelper.Input.IsNewlyPressed(_inputs["Warp to Ship"]))
            {
                WarpTo(SpawnLocation.Ship);
            }
            if (ModHelper.Input.IsNewlyPressed(_inputs["Warp to Quantum Moon"]))
            {
                WarpTo(SpawnLocation.QuantumMoon);
            }
            if (ModHelper.Input.IsNewlyPressed(_inputs["Warp to Attlerock"]))
            {
                WarpTo(SpawnLocation.LunarLookout);
            }
        }

        private void CycleGUIMode()
        {
            _renderValue++;
            if (_renderValue >= 8)
            {
                _renderValue = 0;
            }
            ModHelper.Console.WriteLine("Render value: " + _renderValue);
            typeof(GUIMode).GetAnyField("_renderMode").SetValue(null, _renderValue);
        }

        private void WarpTo(SpawnLocation location)
        {
            ModHelper.Console.WriteLine($"Warping to {location}!");
            _playerSpawner.DebugWarp(_playerSpawner.GetSpawnPoint(location));
        }

    }
}
