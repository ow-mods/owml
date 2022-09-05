using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OWML.Common;
using OWML.Utils;

namespace OWML.Launcher
{
	public class App
	{
		private readonly IOwmlConfig _owmlConfig;
		private readonly IModManifest _owmlManifest;
		private readonly IModConsole _writer;
		private readonly IModFinder _modFinder;
		private readonly IPathFinder _pathFinder;
		private readonly IOWPatcher _owPatcher;
		private readonly IGameVersionHandler _versionHandler;
		private readonly IProcessHelper _processHelper;
		private readonly IArgumentHelper _argumentHelper;

		public App(
			IOwmlConfig owmlConfig,
			IModManifest owmlManifest,
			IModConsole writer,
			IModFinder modFinder,
			IPathFinder pathFinder,
			IOWPatcher owPatcher,
			IGameVersionHandler versionHandler,
			IProcessHelper processHelper,
			IArgumentHelper argumentHelper)
		{
			_owmlConfig = owmlConfig;
			_owmlManifest = owmlManifest;
			_writer = writer;
			_modFinder = modFinder;
			_pathFinder = pathFinder;
			_owPatcher = owPatcher;
			_versionHandler = versionHandler;
			_processHelper = processHelper;
			_argumentHelper = argumentHelper;
		}

		public void Run()
		{
			_writer.WriteLine($"Started OWML v{_owmlManifest.Version}", MessageType.Info);

			LocateGamePath();

			CheckGameVersion();

			CopyGameFiles();

			var mods = _modFinder.GetMods();

			ShowModList(mods);

			_owPatcher.PatchGame();

			ExecutePatchers(mods);

			var hasPortArgument = _argumentHelper.HasArgument(Constants.ConsolePortArgument);

			StartGame();

			if (hasPortArgument)
			{
				ExitConsole();
				return;
			}

			Console.ReadLine();
		}

		private void LocateGamePath()
		{
			var gamePath = _pathFinder.FindGamePath();
			_writer.WriteLine("Game found in " + gamePath);
			if (gamePath != _owmlConfig.GamePath)
			{
				_owmlConfig.GamePath = gamePath;
				JsonHelper.SaveJsonObject(Constants.OwmlConfigFileName, _owmlConfig);
			}
		}

		private void CheckGameVersion()
		{
			_versionHandler.CompareVersions();
		}

		private void CopyGameFiles()
		{
			var filesToCopy = new[] { "UnityEngine.CoreModule.dll", "Assembly-CSharp.dll" };
			foreach (var fileName in filesToCopy)
			{
				try
				{
					File.Copy($"{_owmlConfig.ManagedPath}/{fileName}", fileName, true);
				}
				catch (Exception ex)
				{
					_writer.WriteLine($"Error while copying game file {fileName}: {ex.Message}");
				}
			}
			_writer.WriteLine("Game files copied.");
		}

		private void ShowModList(IList<IModData> mods)
		{
			if (!mods.Any())
			{
				_writer.WriteLine("Warning - No mods found.", MessageType.Warning);
				return;
			}

			_writer.WriteLine("Found mods:");
			foreach (var modData in mods)
			{
				var stateText = modData.Enabled ? "" : "(disabled)";
				var type = modData.Enabled ? MessageType.Message : MessageType.Warning;
				_writer.WriteLine($"* {modData.Manifest.UniqueName} v{modData.Manifest.Version} {stateText}", type);
			}
		}

		private void ExecutePatchers(IEnumerable<IModData> mods)
		{
			_writer.WriteLine("Executing patchers...", MessageType.Debug);
			mods
				.Where(ShouldExecutePatcher)
				.ToList()
				.ForEach(ExecutePatcher);
		}

		private static bool ShouldExecutePatcher(IModData modData) =>
			!string.IsNullOrEmpty(modData.Manifest.Patcher)
			&& modData.Enabled;

		private void ExecutePatcher(IModData modData)
		{
			_writer.WriteLine($"Executing patcher for {modData.Manifest.UniqueName} v{modData.Manifest.Version}", MessageType.Message);

			var domain = AppDomain.CreateDomain(
				$"{modData.Manifest.UniqueName}.Patcher",
				AppDomain.CurrentDomain.Evidence,
				new AppDomainSetup { ApplicationBase = _owmlConfig.GamePath });

			try
			{
				domain.ExecuteAssembly(
					modData.Manifest.PatcherPath,
					new[] { Path.GetDirectoryName(modData.Manifest.PatcherPath) });
			}
			catch (Exception ex)
			{
				_writer.WriteLine($"Cannot run patcher for mod {modData.Manifest.UniqueName} v{modData.Manifest.Version}: {ex.Message}", MessageType.Error);
			}
			finally
			{
				AppDomain.Unload(domain);
			}
		}

		private void StartGame()
		{
			_argumentHelper.RemoveArgument("consolePort");

			try
			{
				void StartGameViaExe()
					=> _processHelper.Start(_owmlConfig.ExePath, _argumentHelper.Arguments);

				if (_owmlConfig.ForceExe)
				{
					_writer.WriteLine($"Launching game...");
					StartGameViaExe();
					return;
				}

				var gameDll = $"{_owmlConfig.ManagedPath}/Assembly-CSharp.dll";
				var assembly = Assembly.LoadFrom(gameDll);
				var types = assembly.GetTypes();
				var isEpic = types.Any(x => x.Name == "EpicEntitlementRetriever");
				var isSteam = types.Any(x => x.Name == "SteamEntitlementRetriever");
				var isUWP = types.Any(x => x.Name == "MSStoreEntitlementRetriever");

				if (isEpic && !isSteam && !isUWP)
				{
					_writer.WriteLine("Identified as an Epic install. Launching...");
					_processHelper.Start("\"com.epicgames.launcher://apps/starfish%3A601d0668cef146bd8eef75d43c6bbb0b%3AStarfish?action=launch&silent=true\"");
				}
				else if (!isEpic && isSteam && !isUWP)
				{
					_writer.WriteLine("Identified as a Steam install. Launching...");
					_processHelper.Start("steam://rungameid/753640");
				}
				else if (!isEpic && !isSteam && isUWP)
				{
					_writer.WriteLine("Identified as an Xbox Game Pass install. Launching...");
					StartGameViaExe();
				}
				else
				{
					// This should be impossible to get to?!
					_writer.WriteLine("This game isn't from Epic, Steam, or the MSStore...? Wha?\r\n" +
						"Either this game is really specifically corrupted, or you're running Outer Wilds from a new exciting vendor.\r\n" +
						"In any case, this has a 0% chance of appearing in normal use (right now), so I can make this message as long as I want.\r\n" +
						"Suck it, command window! You bend to my will now!\r\n" +
						"Though, if you *are* using Steam, Epic, or Game Pass, and you're seeing this... please let us know! Because you aren't meant to see this. :P\r\n" +
						"Anyway, back to scheduled programming. Launching...");
					StartGameViaExe();
				}
			}
			catch (ReflectionTypeLoadException ex)
			{
				_writer.WriteLine($"ReflectionTypeLoadException while starting game: {ex}\n" +
								   "Top 5 LoaderExceptions:\n" +
								   $"* {string.Join("\n* ", ex.LoaderExceptions.Take(5).ToList().Select(e => e.ToString()).ToArray())}", MessageType.Error);
			}
			catch (Exception ex)
			{
				_writer.WriteLine($"Error while starting game: {ex}", MessageType.Error);
			}
		}

		private void ExitConsole() =>
			_processHelper.KillCurrentProcess();
	}
}
