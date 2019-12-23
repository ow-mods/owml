using UnityEngine;

namespace OWML.Common
{
    public interface IModAssets
    {
        GameObject Create3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename);
    }
}
