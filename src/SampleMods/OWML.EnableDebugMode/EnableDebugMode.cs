using OWML.ModHelper;
using UnityEngine;
using UnityEngine.InputSystem;
using OWML.Common;
using OWML.Utils;

namespace OWML.EnableDebugMode
{
	public class EnableDebugMode : ModBehaviour
	{
		private int _renderValue;
		private bool _isStarted;
		private bool _debugOn;
		private PlayerSpawner _playerSpawner;

		public void Start()
		{
			ModHelper.Console.WriteLine($"In {nameof(EnableDebugMode)}!", MessageType.Info);
			ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
			ModHelper.Events.Subscribe<PlayerSpawner>(Events.AfterAwake);
			ModHelper.Events.Event += OnEvent;
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			if (behaviour is PlayerSpawner playerSpawner && ev == Events.AfterAwake)
			{
				ModHelper.Console.WriteLine("Player spawner loaded!");
				_playerSpawner = playerSpawner;
				_isStarted = true;
			}
		}

		public void Update()
		{
			if (!_isStarted)
			{
				return;
			}

			if (Keyboard.current[Key.Pause].wasPressedThisFrame)
			{
				ToogleDebug();
			}

			if (Keyboard.current[Key.F1].wasPressedThisFrame)
			{
				CycleGUIMode();
			}

			HandleWarping();
		}

		private void HandleWarping()
		{
			if (_debugOn && Keyboard.current[Key.Minus].isPressed)
			{
				return;
			}

			if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.Comet);
			}
			if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.HourglassTwin_1);
			}
			if (Keyboard.current[Key.Numpad3].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.TimberHearth);
			}
			if (Keyboard.current[Key.Numpad4].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.BrittleHollow);
			}
			if (Keyboard.current[Key.Numpad5].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.GasGiant);
			}
			if (Keyboard.current[Key.Numpad6].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.DarkBramble);
			}
			if (Keyboard.current[Key.Numpad0].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.Ship);
			}
			if (Keyboard.current[Key.Numpad7].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.QuantumMoon);
			}
			if (Keyboard.current[Key.Numpad8].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.LunarLookout);
			}
			if (Keyboard.current[Key.Numpad9].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.InvisiblePlanet);
			}
		}

		private void CycleGUIMode()
		{
			_renderValue++;
			if (_renderValue > 8)
			{
				_renderValue = 0;
			}
			ModHelper.Console.WriteLine("Render value: " + (GUIMode.RenderMode)_renderValue);
			GUIMode.SetRenderMode((GUIMode.RenderMode)_renderValue);
		}

		private void ToogleDebug()
		{
			_debugOn = !_debugOn;
			var debug = FindObjectOfType<DebugInputManager>();
			debug.SetValue("_debugInputMode", _debugOn ? 1 : 0);
		}

		private void WarpTo(SpawnLocation location)
		{
			ModHelper.Console.WriteLine($"Warping to {location}!");
			_playerSpawner.DebugWarp(_playerSpawner.GetSpawnPoint(location));
		}
	}
}