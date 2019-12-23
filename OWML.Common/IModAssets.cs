using UnityEngine;

namespace OWML.Common
{
    public interface IModAssets
    {
        GameObject Load3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename);
        MeshFilter LoadMesh(ModBehaviour modBehaviour, string objectFilename);
        MeshRenderer LoadTexture(ModBehaviour modBehaviour, string imageFilename);
        AudioSource LoadAudio(ModBehaviour modBehaviour, string audioFilename);
    }
}
