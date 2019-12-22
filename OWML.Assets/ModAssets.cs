using System.Collections;
using OWML.Common;
using UnityEngine;

namespace OWML.Assets
{
    public class ModAssets : IModAssets
    {
        private readonly IModConsole _console;
        private readonly ObjImporter _objImporter;

        public ModAssets(IModConsole console)
        {
            _console = console;
            _objImporter = new ObjImporter();
        }

        public GameObject Create3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;

            _console.WriteLine("Creating object from " + objectPath);

            var go = new GameObject();

            var mesh = _objImporter.ImportFile(objectPath);

            var meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            var loadTexture = LoadTexture(go, imagePath);
            modBehaviour.StartCoroutine(loadTexture);

            return go;
        }

        private IEnumerator LoadTexture(GameObject go, string imagePath)
        {
            var texture = new Texture2D(4, 4, TextureFormat.DXT1, false);
            var url = "file://" + imagePath;
            using (var www = new WWW(url))
            {
                yield return www;
                www.LoadImageIntoTexture(texture);
            }
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = texture;
        } 

    }
}
