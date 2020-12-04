using System;
using System.Collections.Generic;
using UnityEngine;
using OWML.Common.Interfaces;

namespace OWML.ModHelper.Input
{
    public class ModInputTextures : IModInputTextures
    {
        private Dictionary<string, Texture2D> _loadedTextures;

        internal void FillTextureLibrary()
        {
            _loadedTextures = new Dictionary<string, Texture2D>();
            var config = OWInput.GetActivePadConfig() ?? InputUtil.GamePadConfig_Xbox;
            for (var code = ModInputLibrary.MinUsefulKey; code < ModInputLibrary.MaxUsefulKey; code++)
            {
                var key = (KeyCode)code;
                if (!Enum.IsDefined(typeof(KeyCode), key))
                {
                    continue;
                }
                var keyName = ModInputLibrary.KeyCodeToString(key);
                if (_loadedTextures.ContainsKey(keyName))
                {
                    continue;
                }
                var button = InputTranslator.ConvertKeyCodeToButton(key, config);
                var toStore = (int)key >= ModInputLibrary.MinGamepadKey ?
                    ButtonPromptLibrary.SharedInstance.GetButtonTexture(button) :
                    ButtonPromptLibrary.SharedInstance.GetButtonTexture(key);
                _loadedTextures.Add(keyName, toStore);
            }
        }

        public Texture2D KeyTexture(string key)
        {
            return KeyTexture(ModInputLibrary.StringToKeyCode(key));
        }

        public Texture2D KeyTexture(KeyCode key)
        {
            if (_loadedTextures == null)
            {
                FillTextureLibrary();
            }
            var keyName = ModInputLibrary.KeyCodeToString(key);
            if (_loadedTextures.ContainsKey(keyName))
            {
                return _loadedTextures[keyName];
            }
            var config = OWInput.GetActivePadConfig() ?? InputUtil.GamePadConfig_Xbox;
            var toStore = (int)key >= ModInputLibrary.MinGamepadKey ?
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(InputTranslator.ConvertKeyCodeToButton(key, config)) :
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(key);
            _loadedTextures.Add(keyName, toStore);
            return toStore;
        }
    }
}