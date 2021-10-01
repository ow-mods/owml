using System.IO;
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
			var modAsset = new GameObject().AddComponent<ObjectAsset>();
			
			var meshFilter = modAsset.AddComponent<MeshFilter>();
			meshFilter.mesh = GetMesh(objectFilename);
			modAsset.SetMeshFilter(meshFilter);
			
			var meshRenderer = modAsset.AddComponent<MeshRenderer>();
			meshRenderer.material.mainTexture = GetTexture(imageFilename);
			modAsset.SetMeshRenderer(meshRenderer);
			return modAsset;
		}

		public IModAsset<MeshFilter> LoadMesh(string filename)
		{
			var modAsset = new GameObject().AddComponent<MeshAsset>();
			var meshFilter = modAsset.AddComponent<MeshFilter>();
			meshFilter.mesh = GetMesh(filename);
			modAsset.SetAsset(meshFilter);
			return modAsset;
		}

		public IModAsset<MeshRenderer> LoadTexture(string filename)
		{
			var modAsset = new GameObject().AddComponent<TextureAsset>();
			var meshRenderer = modAsset.AddComponent<MeshRenderer>();
			meshRenderer.material.mainTexture = GetTexture(filename);
			modAsset.SetAsset(meshRenderer);
			return modAsset;
		}

		public IModAsset<AudioSource> LoadAudio(string audioFilename)
		{
			var modAsset = new GameObject().AddComponent<AudioAsset>();
			var audioSource = modAsset.AddComponent<AudioSource>();
			audioSource.clip = GetAudio(audioFilename);
			modAsset.SetAsset(audioSource);
			return modAsset;
		}

		public Mesh GetMesh(string filename)
		{
			var path = _manifest.ModFolderPath + filename;
			_console.WriteLine($"Loading mesh from {path}");
			return _objImporter.ImportFile(path);
		}

		public Texture2D GetTexture(string filename)
		{
			var path = _manifest.ModFolderPath + filename;
			_console.WriteLine($"Loading texture from {path}");
			var data = File.ReadAllBytes(path);
			var texture = new Texture2D(2, 2);
			texture.LoadRawTextureData(data);
			return texture;
		}

		public GameObject Get3DObject(string objectFilename, string imageFilename)
		{
			var go = new GameObject();
			go.AddComponent<MeshFilter>().mesh = GetMesh(objectFilename);
			go.AddComponent<MeshRenderer>().material.mainTexture = GetTexture(imageFilename);
			return go;
		}

		public AudioClip GetAudio(string filename)
		{
			var path = _manifest.ModFolderPath + filename;
			_console.WriteLine($"Loading audio from {path}");
			using var reader = new AudioFileReader(path);
			var outputBytes = new float[reader.Length];
			reader.Read(outputBytes, 0, (int)reader.Length);
			var clip = AudioClip.Create(path, (int)reader.Length, reader.WaveFormat.Channels, reader.WaveFormat.SampleRate, false);
			clip.SetData(outputBytes, 0);
			return clip;
		}
	}
}