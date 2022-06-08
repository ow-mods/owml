using Newtonsoft.Json;

namespace OWML.Common
{
	public interface IModStorage
	{
		T Load<T>(string filename);

		T Load<T>(string filename, bool fixBackslashes, JsonSerializerSettings settings = null);

		void Save<T>(T obj, string filename);
	}
}
