using UnityEngine;

namespace OWML.Common
{
    public interface IModAssets
    {
        ModAsset<GameObject> Load3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename);
        ModAsset<MeshFilter> LoadMesh(ModBehaviour modBehaviour, string objectFilename);
        ModAsset<MeshRenderer> LoadTexture(ModBehaviour modBehaviour, string imageFilename);
        ModAsset<AudioSource> LoadAudio(ModBehaviour modBehaviour, string audioFilename);
    }
}
