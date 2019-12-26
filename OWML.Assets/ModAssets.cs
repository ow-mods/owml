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

        public IModAsset<GameObject> Load3DObject(IModBehaviour modBehaviour, string objectFilename, string imageFilename)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;
            _console.WriteLine("Loading object from " + objectPath);

            var go = new GameObject();
            var modAsset = go.AddComponent<ObjectAsset>();

            modBehaviour.StartCoroutine(LoadMesh(modAsset, objectPath));
            modBehaviour.StartCoroutine(LoadTexture(modAsset, imagePath));

            return modAsset;
        }

        public IModAsset<MeshFilter> LoadMesh(IModBehaviour modBehaviour, string objectFilename)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            _console.WriteLine("Loading mesh from " + objectPath);

            var go = new GameObject();
            var modAsset = go.AddComponent<MeshAsset>();

            modBehaviour.StartCoroutine(LoadMesh(modAsset, objectPath));

            return modAsset;
        }

        public IModAsset<MeshRenderer> LoadTexture(IModBehaviour modBehaviour, string imageFilename)
        {
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;
            _console.WriteLine("Loading texture from " + imagePath);

            var go = new GameObject();
            var modAsset = go.AddComponent<TextureAsset>();

            modBehaviour.StartCoroutine(LoadTexture(modAsset, imagePath));

            return modAsset;
        }

        public IModAsset<AudioSource> LoadAudio(IModBehaviour modBehaviour, string audioFilename)
        {
            var audioPath = modBehaviour.ModManifest.FolderPath + audioFilename;
            _console.WriteLine("Loading audio from " + audioPath);

            var go = new GameObject();
            var modAsset = go.AddComponent<AudioAsset>();

            var loadAudioFrom = audioFilename.EndsWith(".mp3")
                ? LoadAudioFromMp3(modAsset, audioPath)
                : LoadAudioFromWav(modAsset, audioPath);
            modBehaviour.StartCoroutine(loadAudioFrom);

            return modAsset;
        }

        private IEnumerator LoadMesh(ObjectAsset modAsset, string objectPath)
        {
            var mesh = _objImporter.ImportFile(objectPath);
            var meshFilter = modAsset.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            yield return new WaitForEndOfFrame();
            modAsset.SetAsset(modAsset.gameObject);
        }

        private IEnumerator LoadTexture(ObjectAsset modAsset, string imagePath)
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
            var meshRenderer = modAsset.AddComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = texture;
        }

        private IEnumerator LoadMesh(MeshAsset modAsset, string objectPath)
        {
            var mesh = _objImporter.ImportFile(objectPath);
            if (mesh == null)
            {
                _console.WriteLine("Mesh is null");
            }
            var meshFilter = modAsset.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            yield return new WaitForEndOfFrame();
            modAsset.SetAsset(meshFilter);
        }

        private IEnumerator LoadTexture(TextureAsset modAsset, string imagePath)
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
            var meshRenderer = modAsset.AddComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = texture;
            yield return new WaitForEndOfFrame();
            modAsset.SetAsset(meshRenderer);
        }

        private IEnumerator LoadAudioFromMp3(AudioAsset modAsset, string audioPath)
        {
            AudioClip clip;
            using (var reader = new AudioFileReader(audioPath))
            {
                var outputBytes = new float[reader.Length];
                reader.Read(outputBytes, 0, (int)reader.Length);
                clip = AudioClip.Create(audioPath, (int)reader.Length, reader.WaveFormat.Channels, reader.WaveFormat.SampleRate, false);
                clip.SetData(outputBytes, 0);
            }
            var audioSource = modAsset.AddComponent<AudioSource>();
            audioSource.clip = clip;
            yield return new WaitForEndOfFrame();
            modAsset.SetAsset(audioSource);
        }

        private IEnumerator LoadAudioFromWav(AudioAsset modAsset, string audioPath)
        {
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
            var audioSource = modAsset.AddComponent<AudioSource>();
            audioSource.clip = clip;
            yield return new WaitForEndOfFrame();
            modAsset.SetAsset(audioSource);
        }

    }
}
