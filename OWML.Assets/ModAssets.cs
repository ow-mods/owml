using System;
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

        public void Load3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename, Action<GameObject> onLoaded)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;
            _console.WriteLine("Loading object from " + objectPath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();

            modBehaviour.StartCoroutine(LoadMesh(go, objectPath, onLoaded));
            modBehaviour.StartCoroutine(LoadTexture(go, imagePath));
        }

        public void LoadMesh(ModBehaviour modBehaviour, string objectFilename, Action<MeshFilter> onLoaded)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            _console.WriteLine("Loading mesh from " + objectPath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();

            modBehaviour.StartCoroutine(LoadMesh(go, objectPath, onLoaded));
        }

        public void LoadTexture(ModBehaviour modBehaviour, string imageFilename, Action<MeshRenderer> onLoaded)
        {
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;
            _console.WriteLine("Loading texture from " + imagePath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();

            modBehaviour.StartCoroutine(LoadTexture(go, imagePath, onLoaded));
        }

        public void LoadAudio(ModBehaviour modBehaviour, string audioFilename, Action<AudioSource> onLoaded)
        {
            var audioPath = modBehaviour.ModManifest.FolderPath + audioFilename;
            _console.WriteLine("Loading audio from " + audioPath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();

            var loadAudioFrom = audioFilename.EndsWith(".mp3")
                ? LoadAudioFromMp3(go, audioPath, onLoaded)
                : LoadAudioFromWav(go, audioPath, onLoaded);
            modBehaviour.StartCoroutine(loadAudioFrom);
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

        private IEnumerator LoadMesh(GameObject go, string objectPath, Action<MeshFilter> onLoaded)
        {
            var mesh = _objImporter.ImportFile(objectPath);
            if (mesh == null)
            {
                _console.WriteLine("Mesh is null");
            }
            var meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            onLoaded(meshFilter);
            yield return null;
        }

        private IEnumerator LoadMesh(GameObject go, string objectPath, Action<GameObject> onLoaded)
        {
            var mesh = _objImporter.ImportFile(objectPath);
            if (mesh == null)
            {
                _console.WriteLine("Mesh is null");
            }
            var meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            onLoaded(go);
            yield return null;
        }

        private IEnumerator LoadTexture(GameObject go, string imagePath, Action<MeshRenderer> onLoaded)
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
            onLoaded(meshRenderer);
        }

        private IEnumerator LoadAudioFromMp3(GameObject go, string audioPath, Action<AudioSource> onLoaded)
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
            var audioSource = go.AddComponent<AudioSource>();
            audioSource.clip = clip;
            onLoaded(audioSource);
            yield return null;
        }

        private IEnumerator LoadAudioFromWav(GameObject go, string audioPath, Action<AudioSource> onLoaded)
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
            var audioSource = go.AddComponent<AudioSource>();
            audioSource.clip = clip;
            onLoaded(audioSource);
        }

    }
}
