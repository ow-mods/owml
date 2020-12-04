using UnityEngine;

namespace OWML.Common.Interfaces
{
    public interface IModInputTextures
    {
        Texture2D KeyTexture(string key);
        Texture2D KeyTexture(KeyCode key);
    }
}
