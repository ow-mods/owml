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
	}
}
