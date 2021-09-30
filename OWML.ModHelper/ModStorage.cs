using OWML.Common;
using OWML.Utils;

namespace OWML.ModHelper
{
	public class ModStorage : IModStorage
	{
		private readonly IModManifest _manifest;

		public ModStorage(IModManifest manifest) =>
			_manifest = manifest;

		public T Load<T>(string filename) =>
			JsonHelper.LoadJsonObject<T>(_manifest.ModFolderPath + filename);

		public void Save<T>(T obj, string filename) =>
			JsonHelper.SaveJsonObject(_manifest.ModFolderPath + filename, obj);
	}
}