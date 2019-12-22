using OWML.Common;
using OWML.Events;

namespace OWML.EnableDebugMode
{
    public class EnableDebugMode : ModBehaviour
    {
        private int _renderValue;
        private PlayerSpawner _playerSpawner;

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(EnableDebugMode)}!");
            ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
        }

        private void Update()
        {
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
            ModHelper.Console.WriteLine("F1 pressed!");
            _renderValue++;
            if (_renderValue >= 8)
            {
                _renderValue = 0;
            }
            ModHelper.Console.WriteLine("_renderValue: " + _renderValue);
            typeof(GUIMode).GetAnyField("_renderMode").SetValue(null, _renderValue);
        }

        private void WarpTo(SpawnLocation location)
        {
            ModHelper.Console.WriteLine($"Warping to {location}!");
            if (_playerSpawner == null)
            {
                _playerSpawner = FindObjectOfType<PlayerSpawner>();
            }
            _playerSpawner.DebugWarp(_playerSpawner.GetSpawnPoint(location));
        }

    }
}
