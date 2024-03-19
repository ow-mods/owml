using System.Collections.Generic;

namespace OWML.Common
{
	public interface IModDefaultConfig
	{
		bool Enabled { get; set; }

		Dictionary<string, object> Settings { get; set; }

		T GetSettingsValue<T>(string key);

		IModConfig Copy();
	}
}
