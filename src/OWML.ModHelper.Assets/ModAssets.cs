using System.Collections;
using NAudio.Wave;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Assets
{
	public class ModAssets : IModAssets
	{
		private readonly IModConsole _console;
		private readonly IModManifest _manifest;
		private readonly IObjImporter _objImporter;

		public ModAssets(IModConsole console, IModManifest manifest, IObjImporter objImporter)
		{
			_console = console;
			_manifest = manifest;
			_objImporter = objImporter;
		}

		public AssetBundle LoadBundle(string filename)
		{
			var path = _manifest.ModFolderPath + filename;
			_console.WriteLine("Loading asset bundle from " + path);
			var bundle = AssetBundle.LoadFromFile(path);
			if (bundle == null)
			{
				_console.WriteLine("Error - Bundle is null.", MessageType.Error);
			}
			return bundle;
		}

		public IModAsset<GameObject> Load3DObject(string objectFilename, string imageFilename)
		{
			var objectPath = _manifest.ModFolderPath + objectFilename;
			var imagePath = _manifest.ModFolderPath + imageFilename;
			_console.WriteLine($"Loading object from {objectPath}");

			var go = new GameObject();
			var modAsset = go.AddComponent<ObjectAsset>();

			modAsset.StartCoroutine(LoadMesh(modAsset, objectPath));
			modAsset.StartCoroutine(LoadTexture(modAsset, imagePath));

			return modAsset;
		}

		public IModAsset<MeshFilter> LoadMesh(string objectFilename)
		{
			var objectPath = _manifest.ModFolderPath + objectFilename;
			_console.WriteLine($"Loading mesh from {objectPath}");

			var go = new GameObject();
			var modAsset = go.AddComponent<MeshAsset>();

			modAsset.StartCoroutine(LoadMesh(modAsset, objectPath));

			return modAsset;
		}

		public IModAsset<MeshRenderer> LoadTexture(string imageFilename)
		{
			var imagePath = _manifest.ModFolderPath + imageFilename;
			_console.WriteLine($"Loading texture from {imagePath}");

			var go = new GameObject();
			var modAsset = go.AddComponent<TextureAsset>();

			modAsset.StartCoroutine(LoadTexture(modAsset, imagePath));

			return modAsset;
		}

		public IModAsset<AudioSource> LoadAudio(string audioFilename)
		{
			var audioPath = _manifest.ModFolderPath + audioFilename;
			_console.WriteLine($"Loading audio from {audioPath}");

			var go = new GameObject();
			var modAsset = go.AddComponent<AudioAsset>();

			var loadAudioFrom = audioFilename.EndsWith(".mp3")
				? LoadAudioFromMp3(modAsset, audioPath)
				: LoadAudioFromWav(modAsset, audioPath);
			modAsset.StartCoroutine(loadAudioFrom);

			return modAsset;
		}

		private IEnumerator LoadMesh(ObjectAsset modAsset, string objectPath)
		{
			var mesh = _objImporter.ImportFile(objectPath);
			var meshFilter = modAsset.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			yield return new WaitForEndOfFrame();
			modAsset.SetMeshFilter(meshFilter);
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
				_console.WriteLine("Error - Texture is null.", MessageType.Error);
			}
			var meshRenderer = modAsset.AddComponent<MeshRenderer>();
			meshRenderer.material.mainTexture = texture;
			modAsset.SetMeshRenderer(meshRenderer);
		}

		private IEnumerator LoadMesh(MeshAsset modAsset, string objectPath)
		{
			var mesh = _objImporter.ImportFile(objectPath);
			if (mesh == null)
			{
				_console.WriteLine("Error - Mesh is null.", MessageType.Error);
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
				_console.WriteLine("Error - Texture is null.", MessageType.Error);
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
				_console.WriteLine("Error - Audio is null.", MessageType.Error);
			}

			var audioSource = modAsset.AddComponent<AudioSource>();
			audioSource.clip = clip;
			yield return new WaitForEndOfFrame();
			modAsset.SetAsset(audioSource);
		}
	}
}
