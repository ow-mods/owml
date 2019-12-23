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

        public GameObject Load3DObject(ModBehaviour modBehaviour, string objectFilename, string imageFilename)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;
            _console.WriteLine("Loading object from " + objectPath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();

            modBehaviour.StartCoroutine(LoadMesh(meshFilter, objectPath));
            modBehaviour.StartCoroutine(LoadTexture(meshRenderer, imagePath));

            return go;
        }

        public MeshFilter LoadMesh(ModBehaviour modBehaviour, string objectFilename)
        {
            var objectPath = modBehaviour.ModManifest.FolderPath + objectFilename;
            _console.WriteLine("Loading mesh from " + objectPath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();
            var meshFilter = go.AddComponent<MeshFilter>();

            modBehaviour.StartCoroutine(LoadMesh(meshFilter, objectPath));

            return meshFilter;
        }

        public MeshRenderer LoadTexture(ModBehaviour modBehaviour, string imageFilename)
        {
            var imagePath = modBehaviour.ModManifest.FolderPath + imageFilename;
            _console.WriteLine("Loading texture from " + imagePath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();
            var meshRenderer = go.AddComponent<MeshRenderer>();

            modBehaviour.StartCoroutine(LoadTexture(meshRenderer, imagePath));

            return meshRenderer;
        }

        public AudioSource LoadAudio(ModBehaviour modBehaviour, string audioFilename)
        {
            var audioPath = modBehaviour.ModManifest.FolderPath + audioFilename;
            _console.WriteLine("Loading audio from " + audioPath);

            var go = new GameObject();
            go.AddComponent<DontDestroyOnLoad>();
            var audioSource = go.AddComponent<AudioSource>();

            var routine = audioFilename.EndsWith(".mp3")
                ? LoadAudioFromMp3(audioSource, audioPath)
                : LoadAudioFromWav(audioSource, audioPath);
            modBehaviour.StartCoroutine(routine);
            return audioSource;
        }

        private IEnumerator LoadMesh(MeshFilter meshFilter, string objectPath)
        {
            var mesh = _objImporter.ImportFile(objectPath);
            if (mesh == null)
            {
                _console.WriteLine("Mesh is null");
            }
            meshFilter.mesh = mesh;
            yield return null;
        }

        private IEnumerator LoadTexture(MeshRenderer meshRenderer, string imagePath)
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
            meshRenderer.material.mainTexture = texture;
        }

        private IEnumerator LoadAudioFromMp3(AudioSource audioSource, string audioPath)
        {
            _console.WriteLine("Loading mp3");
            AudioClip audioClip;
            using (var reader = new AudioFileReader(audioPath))
            {
                var outputBytes = new float[reader.Length];
                _console.WriteLine("Length: " + reader.Length);
                reader.Read(outputBytes, 0, (int)reader.Length);
                audioClip = AudioClip.Create(audioPath, (int)reader.Length, reader.WaveFormat.Channels,
                    reader.WaveFormat.SampleRate, false);
                audioClip.SetData(outputBytes, 0);
            }
            audioSource.clip = audioClip;
            yield return null;
        }

        private IEnumerator LoadAudioFromWav(AudioSource audioSource, string audioPath)
        {
            _console.WriteLine("Loading wav");
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
