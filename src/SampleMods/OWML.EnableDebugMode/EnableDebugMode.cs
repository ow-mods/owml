using OWML.ModHelper;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using OWML.Common;
using OWML.Utils;

namespace OWML.EnableDebugMode
{
	public static class ClearPatch
    {
		public static bool prefP()
        {
			return false;
		}
    }
	public class EnableDebugMode : ModBehaviour
	{
		private int _renderValue;
		private bool _isStarted;
		private PlayerSpawner _playerSpawner;

		public override void Configure(IModConfig config)
		{
			//foreach (var input in _inputs)
			//{
			//ModHelper.Input.UnregisterCombination(input.Value);
			//}
			foreach (var key in config.Settings.Keys)
			{
				var value = config.GetSettingsValue<string>(key);
				if (!string.IsNullOrEmpty(value))
				{
					//_inputs[key] = ModHelper.Input.RegisterCombination(this, key, value);
				}
			}
		}

		public void Start()
		{
			ModHelper.Console.WriteLine($"In {nameof(EnableDebugMode)}!", MessageType.Info);
			//ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
			ModHelper.HarmonyHelper.AddPrefix<DebugInputManager>("Awake", typeof(ClearPatch), "prefP");
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
				if (Keyboard.current == null)
				{
					ModHelper.Console.WriteLine("Keyboard is null!", MessageType.Error);
				}
				if (Mouse.current == null)
				{
					ModHelper.Console.WriteLine("Mouse is null!", MessageType.Error);
				}
			}
		}

		public void Update()
		{
			if (!_isStarted)
			{
				return;
			}
			if (Keyboard.current!=null&&Keyboard.current[Key.F1].wasPressedThisFrame)
			{
				CycleGUIMode();
			}
			

			HandleWarping();

			//TestUnpatching();
		}

		private void HandleWarping()
		{
			
			if (Keyboard.current!=null&&Keyboard.current[Key.Numpad1].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.Comet);
			}
			if (Keyboard.current != null && Keyboard.current[Key.Numpad2].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.HourglassTwin_1);
			}
			if (Keyboard.current != null && Keyboard.current[Key.Numpad3].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.TimberHearth);
			}
			if (Keyboard.current != null && Keyboard.current[Key.Numpad4].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.BrittleHollow);
			}
			if (Keyboard.current != null && Keyboard.current[Key.Numpad5].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.GasGiant);
			}
			if (Keyboard.current != null && Keyboard.current[Key.Numpad6].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.DarkBramble);
			}
			if (Keyboard.current != null && Keyboard.current[Key.Numpad0].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.Ship);
			}
			if (Keyboard.current != null && Keyboard.current[Key.Numpad7].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.QuantumMoon);
			}
			if (Keyboard.current != null && Keyboard.current[Key.Numpad8].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.LunarLookout);
			}
			if (Keyboard.current != null && Keyboard.current[Key.Numpad9].wasPressedThisFrame)
			{
				WarpTo(SpawnLocation.InvisiblePlanet);
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
			typeof(GUIMode).GetAnyMember("_renderMode").SetValue(null, _renderValue);
		}

		private void WarpTo(SpawnLocation location)
		{
			ModHelper.Console.WriteLine($"Warping to {location}!");
			_playerSpawner.DebugWarp(_playerSpawner.GetSpawnPoint(location));
		}
	}
}