using UnityEngine;

namespace OWML.Common
{
    public interface IModAssets
    {
        IModAsset<GameObject> Load3DObject(IModBehaviour modBehaviour, string objectFilename, string imageFilename);
        IModAsset<MeshFilter> LoadMesh(IModBehaviour modBehaviour, string objectFilename);
        IModAsset<MeshRenderer> LoadTexture(IModBehaviour modBehaviour, string imageFilename);
        IModAsset<AudioSource> LoadAudio(IModBehaviour modBehaviour, string audioFilename);
    }
}
