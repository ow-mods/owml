using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModInputCombination : IModInputCombination
    {
        private const int MaxUsefulKey = 350;
        private const int MaxComboLength = 7;
        private const string XboxPrefix = "xbox_";

        public float LastPressedMoment { get; private set; }
        public bool IsFirst { get; private set; }
        public float PressDuration => LastPressedMoment - _firstPressedMoment;
        public string ModName { get; }
        public string Name { get; }
        public string FullName => ModName + "." + Name;
        public ReadOnlyCollection<KeyCode> Singles => _singles.AsReadOnly();
        public ReadOnlyCollection<long> Hashes => _hashes.AsReadOnly();

        private bool _isPressed;
        private float _firstPressedMoment;
        private List<KeyCode> _singles = new List<KeyCode>();
        private List<long> _hashes = new List<long>();

        private KeyCode StringToKeyCodeKeyboard(string keyboardKey)
        {
            if (keyboardKey == "control" || keyboardKey == "ctrl")
            {
                return KeyCode.LeftControl;
            }
            if (keyboardKey == "shift")
            {
                return KeyCode.LeftShift;
            }
            if (keyboardKey == "alt")
            {
                return KeyCode.LeftAlt;
            }
            var code = (KeyCode)Enum.Parse(typeof(KeyCode), keyboardKey, true);
            return Enum.IsDefined(typeof(KeyCode), code) ? code : KeyCode.None;
        }

        private KeyCode StringToKeyCodeGamepad(string gamepadKey)
        {
            var gamepadcodeCode = (JoystickButton)Enum.Parse(typeof(JoystickButton), gamepadKey, true);
            return (Enum.IsDefined(typeof(JoystickButton), gamepadcodeCode)) ?
                InputTranslator.GetButtonKeyCode(gamepadcodeCode) : KeyCode.None;
        }

        private KeyCode StringToKeyCodeXbox(string xboxKey)
        {
            switch (xboxKey[0])
            {
                case 'A':
                    return InputTranslator.GetButtonKeyCode(JoystickButton.FaceDown);
                case 'B':
                    return InputTranslator.GetButtonKeyCode(JoystickButton.FaceRight);
                case 'X':
                    return InputTranslator.GetButtonKeyCode(JoystickButton.FaceLeft);
                case 'Y':
                    return InputTranslator.GetButtonKeyCode(JoystickButton.FaceUp);
                default:
                    return StringToKeyCodeGamepad(xboxKey);
            }
        }

        private KeyCode StringToKeyCode(string key)
        {
            key = key.Trim();
            return key.Contains(XboxPrefix) ? StringToKeyCodeXbox(key.Substring(XboxPrefix.Length)) 
                : StringToKeyCodeKeyboard(key);
        }

        private int[] StringToKeyArray(string stringCombination)
        {
            var keyCombination = new int[MaxComboLength];
            var i = 0;
            foreach (var key in stringCombination.Trim().ToLower().Split('+'))
            {
                var code = StringToKeyCode(key);
                if (code == KeyCode.None)
                {
                    keyCombination[0] = (int)RegistrationCode.InvalidCombination;
                    return keyCombination;
                }
                if (i >= MaxComboLength)
                {
                    keyCombination[0] = (int)RegistrationCode.CombinationTooLong;
                    return keyCombination;
                }
                keyCombination[i] = (int)code;
                i++;
            }
            Array.Sort(keyCombination);
            return keyCombination;
        }

        private long StringToHash(string stringCombination)
        {
            var keyCombination = StringToKeyArray(stringCombination);
            if (keyCombination[0] < 0)
            {
                return keyCombination[0];
            }
            long hash = 0;
            for (var i = 0; i < MaxComboLength; i++)
            {
                hash = hash * MaxUsefulKey + keyCombination[i];
            }
            return hash;
        }

        private List<long> StringToHashes(string combinations)
        {
            var hashes = new List<long>();
            foreach (var combo in combinations.Split('/'))
            {
                var hash = StringToHash(combo);
                if (hash <= 0)
                {
                    hashes.Clear();
                    hashes.Add(hash);
                    return hashes;
                }
                hashes.Add(hash);
                if (hash < MaxUsefulKey)
                {
                    _singles.Add((KeyCode)hash);
                }    
            }
            return hashes;
        }

        internal ModInputCombination(IModManifest mod, string name, string combination)
        {
            ModName = mod.Name;
            Name = name;
            _hashes = StringToHashes(combination);
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
