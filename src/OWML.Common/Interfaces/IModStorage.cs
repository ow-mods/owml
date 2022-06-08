using Newtonsoft.Json;
using System;

namespace OWML.Common
{
	public interface IModStorage
	{
		[Obsolete]
		T Load<T>(string filename);

		T Load<T>(string filename, bool fixBackslashes = true, JsonSerializerSettings settings = null);

		void Save<T>(T obj, string filename);
	}
}
