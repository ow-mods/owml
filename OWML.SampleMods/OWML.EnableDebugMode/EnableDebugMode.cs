using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.EnableDebugMode
{
    public class EnableDebugMode : ModBehaviour
    {
        private int _renderValue;
        private bool _isStarted;
        private PlayerSpawner _playerSpawner;

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(EnableDebugMode)}!");
            ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
            ModHelper.Events.Subscribe<PlayerSpawner>(Events.AfterAwake);
            ModHelper.Events.Subscribe<Menu>(Events.AfterAwake);
            ModHelper.Events.OnEvent += OnEvent;

            var fooButton = ModHelper.Menus.MainMenu.AddButton("DUCKING FINALLY!", 4);
            fooButton.onClick.AddListener(OnFooClick);

            //var barButton = ModHelper.Menus.PauseMenu.AddButton("Bar");
            //barButton.onClick.AddListener(OnBarClick);
        }

        private void OnFooClick()
        {
            ModHelper.Console.WriteLine("Foo clicked!");
        }

        private void OnBarClick()
        {
            ModHelper.Console.WriteLine("Bar clicked!");
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            var type = behaviour.GetType();
            if (type == typeof(PlayerSpawner) && ev == Events.AfterAwake)
            {
                _playerSpawner = (PlayerSpawner)behaviour;
                _isStarted = true;
            }
            if (type == typeof(Menu) && ev == Events.AfterAwake)
            {
                ModHelper.Console.WriteLine("Awake of Menu");
            }
        }

        private void Update()
        {
            if (!_isStarted)
            {
                return;
            }

            if (Input.GetKeyDown(DebugKeyCode.cycleGUIMode))
            {
                CycleGUIMode();
            }

            if (Input.GetKeyDown(DebugKeyCode.cometWarp))
            {
                WarpTo(SpawnLocation.Comet);
            }
            if (Input.GetKeyDown(DebugKeyCode.hourglassTwinsWarp))
            {
                WarpTo(SpawnLocation.HourglassTwin_1);
            }
            if (Input.GetKeyDown(DebugKeyCode.homePlanetWarp))
            {
                WarpTo(SpawnLocation.TimberHearth);
            }
            if (Input.GetKeyDown(DebugKeyCode.brittleHollowWarp))
            {
                WarpTo(SpawnLocation.BrittleHollow);
            }
            if (Input.GetKeyDown(DebugKeyCode.gasGiantWarp))
            {
                WarpTo(SpawnLocation.GasGiant);
            }
            if (Input.GetKeyDown(DebugKeyCode.darkBrambleWarp))
            {
                WarpTo(SpawnLocation.DarkBramble);
            }
            if (Input.GetKeyDown(DebugKeyCode.shipWarp))
            {
                WarpTo(SpawnLocation.Ship);
            }
            if (Input.GetKeyDown(DebugKeyCode.quantumWarp))
            {
                WarpTo(SpawnLocation.QuantumMoon);
            }
            if (Input.GetKeyDown(DebugKeyCode.moonWarp))
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
