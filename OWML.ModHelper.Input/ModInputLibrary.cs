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

        public static JoystickButton XboxButtonToJoystickButton(string xboxKey)
        {
            switch (xboxKey[0])
            {
                case 'A':
                    return JoystickButton.FaceDown;
                case 'B':
                    return JoystickButton.FaceRight;
                case 'X':
                    return JoystickButton.FaceLeft;
                case 'Y':
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
                    return "A";
                case JoystickButton.FaceRight:
                    return "B";
                case JoystickButton.FaceLeft:
                    return "X";
                case JoystickButton.FaceUp:
                    return "Y";
                default:
                    return key.ToString();
            }
        }

        private static KeyCode StringToKeyCodeKeyboard(string keyboardKey)
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

        public static KeyCode StringToKeyCode(string key)
        {
            return key.Contains(XboxPrefix) ?
               InputTranslator.GetButtonKeyCode(XboxButtonToJoystickButton(key.Substring(XboxPrefix.Length))) :
               StringToKeyCodeKeyboard(key);
        }

        public static string KeyCodeToString(KeyCode key)
        {
            return (int)key >= MinGamepadKey ?
                XboxPrefix + JoystickButtonToXboxButton(InputTranslator.ConvertKeyCodeToButton(key, OWInput.GetActivePadConfig())) :
                key.ToString();
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

        public static Texture2D KeyTexture(string key)
        {
            return key.Contains(XboxPrefix) ?
               ButtonPromptLibrary.SharedInstance.GetButtonTexture(XboxButtonToJoystickButton(key.Substring(XboxPrefix.Length))) :
               ButtonPromptLibrary.SharedInstance.GetButtonTexture((KeyCode)Enum.Parse(typeof(KeyCode), key));
        }

        public static Texture2D KeyTexture(KeyCode key)
        {
            return ((int)key) >= MinGamepadKey ?
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(InputTranslator.ConvertKeyCodeToButton(key, OWInput.GetActivePadConfig())) :
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(key);
        }
    }
}
