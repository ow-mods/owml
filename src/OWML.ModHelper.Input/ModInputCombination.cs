using System.Collections.ObjectModel;
using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
	public class ModInputCombination : IModInputCombination
	{
		public float LastPressedMoment { get; private set; }

		public bool IsFirst { get; private set; }

		public float PressDuration => LastPressedMoment - _firstPressedMoment;

		public string ModName => _manifest.Name;

		public string Name { get; }

		public string FullName => $"{ModName}.{Name}";

		public ReadOnlyCollection<KeyCode> Singles => _singles.AsReadOnly();

		public ReadOnlyCollection<long> Hashes => _hashes.AsReadOnly();

		private bool _isPressed;
		private float _firstPressedMoment;
		private readonly List<KeyCode> _singles = new();
		private readonly List<long> _hashes;
		private readonly IModManifest _manifest;
		private readonly IModConsole _console;

		internal ModInputCombination(IModManifest manifest, IModConsole console, string name, string combination)
		{
			_manifest = manifest;
			_console = console;
			Name = name;
			_hashes = StringToHashes(combination);
		}

		private List<long> StringToHashes(string combinations)
		{
			var hashes = new List<long>();
			foreach (var combo in combinations.Split('/'))
			{
				var hash = ModInputLibrary.StringToHash(combo);
				if (hash <= 0)
				{
					_console.WriteLine($"Warning - Invalid part of combo in {FullName}: {combo}, " +
						ModInputLibrary.GetReadableMessage((RegistrationCode)hash), MessageType.Warning);
					continue;
				}

				hashes.Add(hash);
				if (hash < ModInputLibrary.MaxUsefulKey)
				{
					_singles.Add((KeyCode)hash);
				}
			}
			return hashes;
		}

		public void InternalSetPressed(bool isPressed = true)
		{
			IsFirst = isPressed != _isPressed;
			if (isPressed)
			{
				if (IsFirst)
				{
					_firstPressedMoment = Time.realtimeSinceStartup;
				}
				LastPressedMoment = Time.realtimeSinceStartup;
			}
			_isPressed = isPressed;
		}
	}
}