using System;
using UnityEngine;

namespace OWML.Common
{
	public interface IModAssets
	{
		AssetBundle LoadBundle(string filename);

		GameObject Get3DObject(string objectFilename, string audioFilename);

		Mesh GetMesh(string filename);

		Texture2D GetTexture(string filename);


		AudioClip GetAudio(string filename);
		
		[Obsolete("Use Get3DObject instead.")]
		IModAsset<GameObject> Load3DObject(string objectFilename, string imageFilename);
		
		[Obsolete("Use GetMesh instead.")]
		IModAsset<MeshFilter> LoadMesh(string filename);

		[Obsolete("Use GetTexture instead.")]
		IModAsset<MeshRenderer> LoadTexture(string filename);

		[Obsolete("Use GetAudio instead.")]
		IModAsset<AudioSource> LoadAudio(string filename);
	}
}
