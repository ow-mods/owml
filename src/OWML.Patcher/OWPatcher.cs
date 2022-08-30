using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet.Emit;
using dnpatch;
using OWML.Common;

namespace OWML.Patcher
{
	public class OWPatcher : IOWPatcher
	{
		private readonly IOwmlConfig _owmlConfig;
		private readonly IModConsole _writer;

		private const string PatchClass = "PermanentManager";
		private const string PatchMethod = "Awake";

		public OWPatcher(IOwmlConfig owmlConfig, IModConsole writer)
		{
			_owmlConfig = owmlConfig;
			_writer = writer;
		}

		public void PatchGame()
		{
			CopyOWMLFiles();
			CopyLibFiles();
			PatchAssembly();
		}

		private void CopyOWMLFiles()
		{
			_writer.WriteLine("Copying OWML files...");

			var filesToCopy = new[] {
				"OWML.ModLoader.dll",
				"OWML.Common.dll",
				"OWML.ModHelper.dll",
				"OWML.ModHelper.Events.dll",
				"OWML.ModHelper.Assets.dll",
				"OWML.ModHelper.Input.dll",
				"OWML.ModHelper.Menus.dll",
				"OWML.ModHelper.Interaction.dll",
				"OWML.Logging.dll",
				"OWML.Utils.dll",
				"OWML.Abstractions.dll",
				"0Harmony.dll",
				"Mono.Cecil.dll",
				"Mono.Cecil.Mdb.dll",
				"Mono.Cecil.Pdb.dll",
				"Mono.Cecil.Rocks.dll",
				"MonoMod.RuntimeDetour.dll",
				"MonoMod.Utils.dll",
				"NAudio-Unity.dll",
				"Autofac.dll",
				"System.ValueTuple.dll",
				Constants.OwmlManifestFileName,
				Constants.OwmlConfigFileName,
				Constants.OwmlDefaultConfigFileName,
				Constants.GameVersionsFileName
			};

			CopyFiles(filesToCopy, "", $"{_owmlConfig.ManagedPath}");
		}

		private void CopyLibFiles()
		{
			_writer.WriteLine("Copying replacement OW files...");

			var filesToCopy = new[] {
				"mscorlib.dll"
			};

			CopyFiles(filesToCopy, $"{_owmlConfig.OWMLPath}lib/", $"{_owmlConfig.ManagedPath}");
		}

		private void CopyFiles(string[] filesToCopy, string pathPrefix, string destination)
		{
			var uncopiedFiles = new List<string>();
			foreach (var filename in filesToCopy)
			{
				try
				{
					File.Copy($"{pathPrefix}{filename}", Path.Combine(destination, filename), true);
				}
				catch
				{
					uncopiedFiles.Add(filename);
				}
			}

			if (uncopiedFiles.Any())
			{
				_writer.WriteLine($"Warning - Failed to copy the following files to {destination} :", MessageType.Warning);
				uncopiedFiles.ForEach(file => _writer.WriteLine($"* {file}", MessageType.Warning));
			}
		}

		private void PatchAssembly()
		{
			_writer.WriteLine("Patching OW assembly...");

			var patcher = new dnpatch.Patcher(Path.Combine(_owmlConfig.ManagedPath, "Assembly-CSharp.dll"));

			var target = new Target
			{
				Class = PatchClass,
				Method = PatchMethod
			};

			var instructions = patcher.GetInstructions(target).ToList();
			var patchedInstructions = GetPatchedInstructions(instructions);

			if (patchedInstructions.Count == 1)
			{
				_writer.WriteLine($"- Assembly is already patched.");
				return;
			}

			if (patchedInstructions.Count > 1)
			{
				_writer.WriteLine($"Removing corrupted patch from {PatchClass}.{PatchMethod}.");
				foreach (var patchedInstruction in patchedInstructions)
				{
					instructions.Remove(patchedInstruction);
				}
			}

			var newInstruction = Instruction.Create(OpCodes.Call, patcher.BuildCall(typeof(ModLoader.ModLoader), "LoadMods", typeof(void), new Type[] { }));
			instructions.Insert(instructions.Count - 1, newInstruction);

			target.Instructions = instructions.ToArray();

			Patch(patcher, target);
			Save(patcher);
		}

		private List<Instruction> GetPatchedInstructions(List<Instruction> instructions) =>
			instructions.Where(x => x.Operand != null && x.Operand.ToString().Contains(nameof(ModLoader.ModLoader))).ToList();

		private void Patch(dnpatch.Patcher patcher, Target target)
		{
			try
			{
				patcher.Patch(target);
			}
			catch (Exception ex)
			{
				_writer.WriteLine($"Error while patching: {ex}", MessageType.Error);
				throw;
			}
		}

		private void Save(dnpatch.Patcher patcher)
		{
			try
			{
				patcher.Save(true);
			}
			catch (Exception ex)
			{
				_writer.WriteLine($"Error while saving patched game assembly: {ex}", MessageType.Error);
				throw;
			}
		}
	}
}
