using System;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public static class ModInputLibrary
    {
        public const float ScaleDown = 0.75f;
        public const string XboxPrefix = "xbox_";
        public const int MinUsefulKey = 8;
        public const int MinGamepadKey = 330;
        public const int MaxUsefulKey = 350;
        public const int MaxComboLength = 7;
        public const int GamePadKeyDiff = 20;

        public static KeyCode NormalizeKeyCode(KeyCode key)
        {
            if ((int)key >= MaxUsefulKey)
            {
                key -= ((int)key - MaxUsefulKey + GamePadKeyDiff) / GamePadKeyDiff * GamePadKeyDiff;
            }
            return key;
        }

        public static JoystickButton XboxButtonToJoystickButton(string xboxKey)
        {
            switch (xboxKey[0])
            {
                case 'a':
                    return JoystickButton.FaceDown;
                case 'b':
                    return JoystickButton.FaceRight;
                case 'x':
                    return JoystickButton.FaceLeft;
                case 'y':
                    return JoystickButton.FaceUp;
                default:
                    var code = (JoystickButton)Enum.Parse(typeof(JoystickButton), xboxKey);
                    return Enum.IsDefined(typeof(JoystickButton), code) ? code : JoystickButton.None;
            }
        }

        public static string JoystickButtonToXboxButton(JoystickButton key)
        {
            switch (key)
            {
                case JoystickButton.FaceDown:
                    return "a";
                case JoystickButton.FaceRight:
                    return "b";
                case JoystickButton.FaceLeft:
                    return "x";
                case JoystickButton.FaceUp:
                    return "y";
                default:
                    return key.ToString();
            }
        }

        private static KeyCode StringToKeyCodeKeyboard(string keyboardKey)
        {
            switch (keyboardKey)
            {
                case "control":
                case "ctrl":
                    return KeyCode.LeftControl;
                case "shift":
                    return KeyCode.LeftShift;
                case "alt":
                    return KeyCode.LeftAlt;
                default:
                    var code = (KeyCode)Enum.Parse(typeof(KeyCode), keyboardKey, true);
                    return Enum.IsDefined(typeof(KeyCode), code) ? code : KeyCode.None;
            }
        }

        private static KeyCode StringToKeyCodeGamepad(string xboxKey)
        {
            var gamepadCode = XboxButtonToJoystickButton(xboxKey);
            return gamepadCode == JoystickButton.None ? KeyCode.None : NormalizeKeyCode(InputTranslator.GetButtonKeyCode(gamepadCode));
        }

        public static KeyCode StringToKeyCode(string key)
        {
            var trimmedKey = key.Trim();
            return trimmedKey.Contains(XboxPrefix) ?
               StringToKeyCodeGamepad(trimmedKey.Substring(XboxPrefix.Length)) :
               StringToKeyCodeKeyboard(trimmedKey);
        }

        private static int[] StringToKeyArray(string stringCombination)
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

        internal static long StringToHash(string stringCombination)
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

        public static string KeyCodeToString(KeyCode key)
        {
            var config = OWInput.GetActivePadConfig() ?? InputUtil.GamePadConfig_Xbox;
            key = NormalizeKeyCode(key);
            return (int)key >= MinGamepadKey ?
                XboxPrefix + JoystickButtonToXboxButton(InputTranslator.ConvertKeyCodeToButton(key, config)) :
                key.ToString();
        }
    }
}