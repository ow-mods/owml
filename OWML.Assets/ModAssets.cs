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

        public GameObject Load3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;
            _console.WriteLine("Loading object from " + objectPath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();

            modBehaviour.StartCoroutine(LoadMesh(go, objectPath));
            modBehaviour.StartCoroutine(LoadTexture(go, imagePath));

            return go;
        }

        public AudioSource LoadAudio(ModBehaviour modBehaviour, string audioFilename)
        {
            var audioPath = modBehaviour.ModManifest.FolderPath + audioFilename;
            _console.WriteLine("Loading audio from " + audioPath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();

            var audioSource = go.AddComponent<AudioSource>();
            modBehaviour.StartCoroutine(LoadAudioClip(audioSource, audioPath));

            return audioSource;
        }

        private IEnumerator LoadMesh(GameObject go, string objectPath)
        {
            var mesh = _objImporter.ImportFile(objectPath);
            if (mesh == null)
            {
                _console.WriteLine("Mesh is null");
            }
            var meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            yield return null;
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
            if (texture == null)
            {
                _console.WriteLine("Texture is null");
            }
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = texture;
        }

        private IEnumerator LoadAudioClip(AudioSource audioSource, string audioPath)
        {
            AudioClip audio;
            var url = "file://" + audioPath;
            using (var www = new WWW(url))
            {
                yield return www;
                audio = www.GetAudioClip(true);
            }
            if (audio == null)
            {
                _console.WriteLine("Audio is null");
            }
            audioSource.clip = audio;
        }

    }
}
