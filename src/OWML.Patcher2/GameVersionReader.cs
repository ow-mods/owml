using System.Text;
using System.Text.RegularExpressions;
using OWML.Common;

namespace OWML.Patcher
{
	public class GameVersionReader : IGameVersionReader
	{
		private const int PlayerSettingsSector = 0;
		private const string VersionPattern = @"\d+(\.\d+){3,}"; // can handle 3-infinite dots

		private readonly IBinaryPatcher _binaryPatcher;
		
		public GameVersionReader(IBinaryPatcher binaryPatcher) => 
			_binaryPatcher = binaryPatcher;

		public string GetGameVersion()
		{
			var sectorBytes = _binaryPatcher.GetSectorBytes(_binaryPatcher.ReadFileBytes(), PlayerSettingsSector);
			var sectorString = Encoding.ASCII.GetString(sectorBytes);
			var match = Regex.Match(sectorString, VersionPattern);
			return match.Success ? match.Value : "not found";
		}
	}
}
