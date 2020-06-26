using System;
using System.Collections.Generic;
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

        private static Dictionary<string, Texture2D> loadedTextures/* = new Dictionary<string,Texture2D>()*/;

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
               NormalizeKeyCode(InputTranslator.GetButtonKeyCode(XboxButtonToJoystickButton(key.Substring(XboxPrefix.Length)))) :
               StringToKeyCodeKeyboard(key);
        }

        public static KeyCode NormalizeKeyCode(KeyCode key)
        {
            if ((int)key >= MaxUsefulKey)
            {
                key -= (((int)key - MaxUsefulKey + GamePadKeyDiff) / GamePadKeyDiff) * GamePadKeyDiff;
            }
            return key;
        }

        public static string KeyCodeToString(KeyCode key)
        {
            var config = OWInput.GetActivePadConfig() ?? InputUtil.GamePadConfig_Xbox;
            key = NormalizeKeyCode(key);
            return ((int)key) >= MinGamepadKey ?
                XboxPrefix + JoystickButtonToXboxButton(InputTranslator.ConvertKeyCodeToButton(key, config)) :
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

        public static void FillTextureLibrary()
        {
            string keyName;
            loadedTextures = new Dictionary<string, Texture2D>();
            KeyCode key;
            var config = OWInput.GetActivePadConfig() ?? InputUtil.GamePadConfig_Xbox;
            int i = 0;
            for (var code = MinUsefulKey; code < MaxUsefulKey; code++)
            {
                key = (KeyCode)code;
                if (!Enum.IsDefined(typeof(KeyCode), key))
                {
                    continue;
                }
                keyName = KeyCodeToString(key);
                if (loadedTextures.ContainsKey(keyName))
                {
                    continue;
                }
                var toStore = ((int)key) >= MinGamepadKey ?
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(InputTranslator.ConvertKeyCodeToButton(key, config)) :
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(key);
                loadedTextures.Add(keyName, toStore);
                i++;
            }
        }

        public static Texture2D KeyTexture(string key)
        {
            if (loadedTextures == null)
            {
                FillTextureLibrary();
            }
            if (loadedTextures.ContainsKey(key))
            {
                return loadedTextures[key];
            }
            else
            {
                var toStore = key.Contains(XboxPrefix) ?
                   ButtonPromptLibrary.SharedInstance.GetButtonTexture(XboxButtonToJoystickButton(key.Substring(XboxPrefix.Length))) :
                   ButtonPromptLibrary.SharedInstance.GetButtonTexture((KeyCode)Enum.Parse(typeof(KeyCode), key));
                loadedTextures.Add(key, toStore);
                return toStore;
            }
        }

        public static Texture2D KeyTexture(KeyCode key)
        {
            if (loadedTextures == null)
            {
                FillTextureLibrary();
            }
            var keyName = KeyCodeToString(key);
            if (loadedTextures.ContainsKey(keyName))
            {
                return loadedTextures[keyName];
            }
            else
            {
                var config = OWInput.GetActivePadConfig() ?? InputUtil.GamePadConfig_Xbox;
                var toStore = ((int)key) >= MinGamepadKey ?
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(InputTranslator.ConvertKeyCodeToButton(key, config)) :
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(key);
                loadedTextures.Add(keyName, toStore);
                return toStore;
            }
        }
    }
}
