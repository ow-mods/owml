using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet.Emit;
using dnpatch;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.Patcher
{
    public class OWPatcher
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
            CopyFiles();
            PatchAssembly();
        }

        private void CopyFiles()
        {
            var filesToCopy = new[] {
                "OWML.ModLoader.dll",
                "OWML.Common.dll",
                "OWML.ModHelper.dll",
                "OWML.ModHelper.Events.dll",
                "OWML.ModHelper.Assets.dll",
                "OWML.ModHelper.Input.dll",
                "OWML.ModHelper.Menus.dll",
                "Newtonsoft.Json.dll",
                "System.Runtime.Serialization.dll",
                "0Harmony.dll",
                "NAudio-Unity.dll",
                "OWML.Manifest.json",
                "OWML.ModHelper.Interaction.dll"
            };
            foreach (var filename in filesToCopy)
            {
                File.Copy(filename, $"{_owmlConfig.ManagedPath}/{filename}", true);
            }
            File.WriteAllText($"{_owmlConfig.ManagedPath}/OWML.Config.json", JsonConvert.SerializeObject(_owmlConfig));
        }

        private void PatchAssembly()
        {
            var patcher = new dnpatch.Patcher($"{_owmlConfig.ManagedPath}/Assembly-CSharp.dll");

            var target = new Target
            {
                Class = PatchClass,
                Method = PatchMethod
            };
            var instructions = patcher.GetInstructions(target).ToList();
            var patchedInstructions = GetPatchedInstructions(instructions);

            if (patchedInstructions.Count == 1)
            {
                _writer.WriteLine($"{PatchClass}.{PatchMethod} is already patched.");
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

            _writer.WriteLine($"Adding patch in {PatchClass}.{PatchMethod}.");

            var newInstruction = Instruction.Create(OpCodes.Call, patcher.BuildCall(typeof(ModLoader.ModLoader), "LoadMods", typeof(void), new Type[] { }));
            instructions.Insert(instructions.Count - 1, newInstruction);

            target.Instructions = instructions.ToArray();

            Patch(patcher, target);

            Save(patcher);
        }

        private List<Instruction> GetPatchedInstructions(List<Instruction> instructions)
        {
            return instructions.Where(x => x.Operand != null && x.Operand.ToString().Contains(nameof(ModLoader.ModLoader))).ToList();
        }

        private void Patch(dnpatch.Patcher patcher, Target target)
        {
            try
            {
                patcher.Patch(target);
            }
            catch (Exception ex)
            {
                _writer.WriteLine("Error while patching: " + ex);
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
                _writer.WriteLine("Error while saving patched game assembly: " + ex);
                throw;
            }
        }

    }
}
