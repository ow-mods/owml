using System;
using UnityEngine;

namespace OWML.Common
{
    public interface IModAssets
    {
        void Load3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename, Action<GameObject> onLoaded);
        void LoadMesh(ModBehaviour modBehaviour, string objectFilename, Action<MeshFilter> onLoaded);
        void LoadTexture(ModBehaviour modBehaviour, string imageFilename, Action<MeshRenderer> onLoaded);
        void LoadAudio(ModBehaviour modBehaviour, string audioFilename, Action<AudioSource> onLoaded);
    }
}
