using Newtonsoft.Json;

namespace OWML.Common
{
	public interface IModStorage
	{
		T Load<T>(string filename, bool fixBackslashes = true, JsonSerializerSettings settings = null);

		void Save<T>(T obj, string filename);
	}
}
