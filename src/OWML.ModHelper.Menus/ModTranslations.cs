using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace OWML.ModHelper.Menus
{
	public class ModTranslations : IModTranslations
	{
		private Dictionary<TextTranslation.Language, Dictionary<string, string>> _translationTable = new();

		private IModManifest _manifest;
		private IModConsole _console;

		// Menu translations are stored under UIDictionary
		// This means OWML config translations follow the New Horizons format
		public static readonly string UIDictionary = nameof(UIDictionary);

		private bool _initialized;

		public ModTranslations(IModManifest manifest, IModConsole console)
		{
			_manifest = manifest;
			_console = console;
		}

		private void Init()
		{
			try
			{
				var translationsFolder = Path.Combine(_manifest.ModFolderPath, "translations");
				foreach (TextTranslation.Language translation in Enum.GetValues(typeof(TextTranslation.Language)))
				{
					var filename = Path.Combine(translationsFolder, $"{translation}.json");
					if (File.Exists(filename))
					{
						var dict = JObject.Parse(File.ReadAllText(filename)).ToObject<Dictionary<string, object>>();
						if (dict.ContainsKey(UIDictionary))
						{
							_translationTable[translation] = (Dictionary<string, string>)(dict[nameof(UIDictionary)] as JObject).ToObject(typeof(Dictionary<string, string>));
						}
					}
				}
				_initialized = true;
			}
			catch (Exception ex)
			{
				_console.WriteLine(ex.ToString(), MessageType.Error);
			}
		}

		public string GetLocalizedString(string key)
		{
			if (!_initialized)
			{
				Init();
			}

			_console.WriteLine($"{_translationTable == null} {string.Join(", ", _translationTable.Keys)}", MessageType.Error);

			try
			{
				if (key == null) return null;
				if (key == string.Empty) return string.Empty;

				if (!_translationTable.TryGetValue(TextTranslation.Get().m_language, out var dict))
				{
					// Default to English
					if (!_translationTable.TryGetValue(TextTranslation.Language.ENGLISH, out dict))
					{
						// Default to key
						return key;
					}
				}

				if (dict.TryGetValue(key, out var value))
				{
					return value;
				}
				else
				{
					return key;
				}
			}
			catch (Exception ex)
			{
				_console.WriteLine(ex.ToString(), MessageType.Error);
				return key;
			}
		}
	}
}
