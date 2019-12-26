namespace OWML.Common
{
    public interface IModAssets
    {
        ObjectAsset Load3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename);
        MeshAsset LoadMesh(ModBehaviour modBehaviour, string objectFilename);
        TextureAsset LoadTexture(ModBehaviour modBehaviour, string imageFilename);
        AudioAsset LoadAudio(ModBehaviour modBehaviour, string audioFilename);
    }
}
