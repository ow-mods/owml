using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;
using dnpatch;
using OWML.Common;

namespace OWML.Patcher
{
    public class ModPatcher
    {
        private readonly IModConfig _config;

        private const string PatchClass = "PermanentManager";
        private const string PatchMethod = "Awake";

        private readonly Dictionary<string, string> _oldPatches = new Dictionary<string, string>
        {
        };

        public ModPatcher(IModConfig config)
        {
            _config = config;
        }

        public void PatchGame()
        {
            var patcher = new dnpatch.Patcher($"{_config.ManagedPath}/Assembly-CSharp.dll");

            RemoveOldPatches(patcher);

            AddCurrentPatch(patcher);

            Save(patcher);
        }

        private void RemoveOldPatches(dnpatch.Patcher patcher)
        {
            foreach (var kvPair in _oldPatches)
            {
                RemovePatch(patcher, kvPair.Key, kvPair.Value);
            }
        }

        private void RemovePatch(dnpatch.Patcher patcher, string className, string methodName)
        {
            var target = new Target
            {
                Class = className,
                Method = methodName
            };
            var instructions = patcher.GetInstructions(target).ToList();

            var patchedInstruction = GetPatchedInstruction(instructions);
            if (patchedInstruction == null)
            {
                Console.WriteLine($"No patch found in {className}.{methodName}.");
                return;
            }

            Console.WriteLine($"Removing old patch found in {className}.{methodName}...");
            instructions.Remove(patchedInstruction);
            target.Instructions = instructions.ToArray();
            patcher.Patch(target);
        }

        private void AddCurrentPatch(dnpatch.Patcher patcher)
        {
            var target = new Target
            {
                Class = PatchClass,
                Method = PatchMethod
            };
            var instructions = patcher.GetInstructions(target).ToList();

            if (IsPatched(instructions))
            {
                Console.WriteLine($"{PatchClass}.{PatchMethod} is already patched.");
                return;
            }

            Console.WriteLine($"Patch not found in {PatchClass}.{PatchMethod}, patching...");

            var newInstruction = Instruction.Create(OpCodes.Call, patcher.BuildCall(typeof(ModLoader.ModLoader), "LoadMods", typeof(void), new Type[] { }));
            instructions.Insert(0, newInstruction);

            target.Instructions = instructions.ToArray();

            Patch(patcher, target);
        }

        private Instruction GetPatchedInstruction(List<Instruction> instructions)
        {
            return instructions.FirstOrDefault(x => x.Operand != null && x.Operand.ToString().Contains(nameof(ModLoader.ModLoader)));
        }

        private bool IsPatched(List<Instruction> instructions)
        {
            var patchInstruction = GetPatchedInstruction(instructions);
            return patchInstruction != null;
        }

        private void Patch(dnpatch.Patcher patcher, Target target)
        {
            try
            {
                patcher.Patch(target);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while patching: " + ex);
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
                Console.WriteLine("Error while saving patched game assembly: " + ex);
                throw;
            }
        }

    }
}
