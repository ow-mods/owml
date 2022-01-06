using System;
using Microsoft.Win32;
using OWML.Common;

namespace OWML.GameFinder
{
	public class UWPGameFinder : BaseFinder
	{
		private const string RegistryPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\Repository\Packages";

		public UWPGameFinder(IOwmlConfig config, IModConsole writer)
			: base(config, writer)
		{
		}

		public override string FindGamePath()
		{
			var appPackages = Registry.CurrentUser.OpenSubKey(RegistryPath);

			var gamePath = "";
			foreach(var appPackageName in appPackages?.GetSubKeyNames())
			{
				var appPackageKey = appPackages.OpenSubKey(appPackageName);
				var packageDisplayName = (string)appPackageKey.GetValue("DisplayName");

				if(!String.IsNullOrEmpty(packageDisplayName) && packageDisplayName.Contains("Outer Wilds"))
				{
					gamePath = (string)appPackageKey.GetValue("PackageRootFolder");
					break;
				}
			}

			if (IsValidGamePath(gamePath))
			{
				return gamePath;
			}

			Writer.WriteLine("Game not found in UWP.");
			return null;
		}
	}
}
