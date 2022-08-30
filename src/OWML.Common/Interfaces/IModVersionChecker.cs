using System;

namespace OWML.Common
{
	public interface IModVersionChecker 
	{
		bool CheckModVersion(IModData data);

		bool CheckModGameVersion(IModData data, Version latestGameVersion);
	}
}