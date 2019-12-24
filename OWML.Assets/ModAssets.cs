using System.Collections;
using NAudio.Wave;
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

        public ModAsset<GameObject> Load3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;
            _console.WriteLine("Loading object from " + objectPath);

            var go = new GameObject();
            var modAsset = go.AddComponent<ModAsset<GameObject>>();
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();

            modBehaviour.StartCoroutine(LoadMesh(meshFilter, objectPath));
            modBehaviour.StartCoroutine(LoadTexture(meshRenderer, imagePath));

            return modAsset;
        }

        public ModAsset<MeshFilter> LoadMesh(ModBehaviour modBehaviour, string objectFilename)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            _console.WriteLine("Loading mesh from " + objectPath);

            var go = new GameObject();
            var modAsset = go.AddComponent<ModAsset<MeshFilter>>();

            modBehaviour.StartCoroutine(LoadMesh(modAsset, objectPath));

            return modAsset;
        }

        public ModAsset<MeshRenderer> LoadTexture(ModBehaviour modBehaviour, string imageFilename)
        {
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;
            _console.WriteLine("Loading texture from " + imagePath);

            var go = new GameObject();
            var modAsset = go.AddComponent<ModAsset<MeshRenderer>>();

            modBehaviour.StartCoroutine(LoadTexture(modAsset, imagePath));

            return modAsset;
        }

        public ModAsset<AudioSource> LoadAudio(ModBehaviour modBehaviour, string audioFilename)
        {
            var audioPath = modBehaviour.ModManifest.FolderPath + audioFilename;
            _console.WriteLine("Loading audio from " + audioPath);

            var go = new GameObject();
            var modAsset = go.AddComponent<ModAsset<AudioSource>>();

            var loadAudioFrom = audioFilename.EndsWith(".mp3")
                ? LoadAudioFromMp3(modAsset, audioPath)
                : LoadAudioFromWav(modAsset, audioPath);
            modBehaviour.StartCoroutine(loadAudioFrom);

            return modAsset;
        }

        private IEnumerator LoadMesh(ModAsset<MeshFilter> modAsset, string objectPath)
        {
            var mesh = _objImporter.ImportFile(objectPath);
            if (mesh == null)
            {
                _console.WriteLine("Mesh is null");
            }
            var meshFilter = modAsset.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            modAsset.Asset = meshFilter;
            yield return null;
        }

        private IEnumerator LoadTexture(ModAsset<MeshRenderer> modAsset, string imagePath)
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
            var meshRenderer = modAsset.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = texture;
            modAsset.Asset = meshRenderer;
        }

        private IEnumerator LoadAudioFromMp3(ModAsset<AudioSource> modAsset, string audioPath)
        {
            _console.WriteLine("Loading mp3");
            AudioClip clip;
            using (var reader = new AudioFileReader(audioPath))
            {
                var outputBytes = new float[reader.Length];
                reader.Read(outputBytes, 0, (int)reader.Length);
                clip = AudioClip.Create(audioPath, (int)reader.Length, reader.WaveFormat.Channels, reader.WaveFormat.SampleRate, false);
                clip.SetData(outputBytes, 0);
            }
            var audioSource = modAsset.gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            modAsset.Asset = audioSource;
            yield return null;
        }

        private IEnumerator LoadAudioFromWav(ModAsset<AudioSource> modAsset, string audioPath)
        {
            _console.WriteLine("Loading wav");
            AudioClip clip;
            var url = "file://" + audioPath;
            using (var www = new WWW(url))
            {
                yield return www;
                clip = www.GetAudioClip(true);
            }
            if (clip == null)
            {
                _console.WriteLine("Audio is null");
            }
            var audioSource = modAsset.gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            modAsset.Asset = audioSource;
        }

    }
}
