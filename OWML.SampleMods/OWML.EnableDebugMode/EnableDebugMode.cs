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
                foreach (string key in _inputs.Keys)
                {
                    ModHelper.Input.UnregisterCombination(_inputs[key]);
                }
            }
            _inputs = new Dictionary<string, IModInputCombination>();
            foreach (string name in config.Settings.Keys)
            {
                if (config.GetSettingsValue<string>(name) != null)
                {
                    var combination = ModHelper.Input.RegisterCombination(this, name, config.GetSettingsValue<string>(name));
                    _inputs.Add(name, combination);
                }
            }
        }

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(EnableDebugMode)}!");
            ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
            ModHelper.Events.Subscribe<PlayerSpawner>(Events.AfterAwake);
            ModHelper.Events.OnEvent += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            var type = behaviour.GetType();
            if (type == typeof(PlayerSpawner) && ev == Events.AfterAwake)
            {
                _playerSpawner = (PlayerSpawner)behaviour;
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
