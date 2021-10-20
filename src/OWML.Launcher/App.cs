using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			_writer.WriteLine("Starting game...");

			_argumentHelper.RemoveArgument("consolePort");

			try
			{
				_processHelper.Start(_owmlConfig.ExePath, _argumentHelper.Arguments);
			}
			catch (Exception ex)
			{
				_writer.WriteLine($"Error while starting game: {ex.Message}", MessageType.Error);
			}
		}

		private void ExitConsole() =>
			_processHelper.KillCurrentProcess();
	}
}
