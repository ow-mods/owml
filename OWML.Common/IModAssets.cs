using UnityEngine;

namespace OWML.Common
{
    public interface IModAssets
    {
        AssetBundle LoadBundle(string filename);
        IModAsset<GameObject> Load3DObject(string objectFilename, string imageFilename);
        IModAsset<MeshFilter> LoadMesh(string objectFilename);
        IModAsset<MeshRenderer> LoadTexture(string imageFilename);
        IModAsset<AudioSource> LoadAudio(string audioFilename);
    }
}
