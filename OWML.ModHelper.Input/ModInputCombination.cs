using System;
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
        public string ModName { get; }
        public string Name { get; }
        public string FullName => $"{ModName}.{Name}";
        public ReadOnlyCollection<KeyCode> Singles => _singles.AsReadOnly();
        public ReadOnlyCollection<long> Hashes => _hashes.AsReadOnly();

        private bool _isPressed;
        private float _firstPressedMoment;
        private List<KeyCode> _singles = new List<KeyCode>();
        private List<long> _hashes = new List<long>();

        internal ModInputCombination(IModManifest mod, string name, string combination)
        {
            ModName = mod.Name;
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
                    hashes.Clear();
                    hashes.Add(hash);
                    return hashes;
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
