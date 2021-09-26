﻿using OWML.ModHelper;
using UnityEngine;
using System.Collections.Generic;
using OWML.Common;
using OWML.Utils;

namespace OWML.EnableDebugMode
{
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
			ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
			ModHelper.Events.Subscribe<PlayerSpawner>(Events.AfterAwake);
			ModHelper.Events.Event += OnEvent;
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			if (behaviour is PlayerSpawner playerSpawner && ev == Events.AfterAwake)
			{
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
			/*
			if (ModHelper.Input.IsNewlyPressed(_inputs["Cycle GUI mode"]))
			{
				CycleGUIMode();
			}
			*/

			HandleWarping();

			TestUnpatching();
		}

		private void HandleWarping()
		{
			/*
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
			*/
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

		private void TestUnpatching()
		{
			if (Input.GetKeyDown(KeyCode.F7))
			{
				ModHelper.Console.WriteLine("Removing Jump");
				ModHelper.HarmonyHelper.EmptyMethod<PlayerCharacterController>("ApplyJump");
			}

			if (Input.GetKeyDown(KeyCode.F8))
			{
				ModHelper.Console.WriteLine("Restoring Jump");
				ModHelper.HarmonyHelper.Unpatch<PlayerCharacterController>("ApplyJump");
			}
		}
	}
}