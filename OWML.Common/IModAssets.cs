using UnityEngine;

namespace OWML.Common
{
    /// <summary>
    /// Helper for loading assets.
    /// </summary>
    public interface IModAssets
    {
        /// <summary>Loads an asset bundle.</summary>
        /// <param name="filename">The name of the asset bundle.</param>
        /// <returns>The asset bundle.</returns>
        AssetBundle LoadBundle(string filename);

        /// <summary>Loads a GameObject.</summary>
        /// <param name="objectFilename">The name of the asset.</param>
        /// <param name="imageFilename"></param>
        /// <returns>The GameObject.</returns>
        IModAsset<GameObject> Load3DObject(string objectFilename, string imageFilename);

        /// <summary>Loads a mesh.</summary>
        /// <param name="objectFilename">The name of the mesh.</param>
        /// <returns>A mesh filter with the mesh attached.</returns>
        IModAsset<MeshFilter> LoadMesh(string objectFilename);

        /// <summary>Loads a texture.</summary>
        /// <param name="imageFilename">The name of the texture.</param>
        /// <returns>A mesh renderer with the texture attached.</returns>
        IModAsset<MeshRenderer> LoadTexture(string imageFilename);

        /// <summary>Loads an audio file.</summary>
        /// <param name="audioFilename">The name of the audio file.</param>
        /// <returns>An AudioSource with the audio file attached.</returns>
        IModAsset<AudioSource> LoadAudio(string audioFilename);
    }
}
