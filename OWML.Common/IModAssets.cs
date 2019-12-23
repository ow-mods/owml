using UnityEngine;

namespace OWML.Common
{
    public interface IModAssets
    {
        GameObject Load3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename);
        AudioSource LoadAudio(ModBehaviour modBehaviour, string audioFilename);
    }
}
