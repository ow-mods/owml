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

        public ModPatcher(IModConfig config)
        {
            _config = config;
        }

        public void PatchGame()
        {
            var patcher = new dnpatch.Patcher($"{_config.ManagedPath}/Assembly-CSharp.dll");
            var target = new Target
            {
                Class = "PermanentManager",
                Method = "Awake"
            };
            var newInstructions = new[]
            {
                Instruction.Create(OpCodes.Call, patcher.BuildCall(typeof(ModLoader.ModLoader), "LoadMods", typeof(void), new Type[] { })),
                Instruction.Create(OpCodes.Ret)
            };
            var instructions = patcher.GetInstructions(target).ToList();
            if (IsPatched(instructions))
            {
                ReplacePatchedInstructions(instructions, newInstructions);
            }
            else
            {
                AddNewInstructions(instructions, newInstructions);
            }
            target.Instructions = instructions.ToArray();
            try
            {
                patcher.Patch(target);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while patching game assembly: " + ex);
                throw;
            }
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

        private void ReplacePatchedInstructions(List<Instruction> instructions, Instruction[] newInstructions)
        {
            Console.WriteLine("Game is already patched. Re-patching.");
            for (var i = 0; i < newInstructions.Length; i++)
            {
                instructions.Remove(instructions.Last());
            }
            instructions.AddRange(newInstructions);
        }

        private void AddNewInstructions(List<Instruction> instructions, Instruction[] newInstructions)
        {
            Console.WriteLine("Game is not patched. Patching!");
            instructions.Remove(instructions.Last());
            instructions.AddRange(newInstructions);
        }

        private bool IsPatched(List<Instruction> instructions)
        {
            return instructions.Any(x => x.Operand != null && x.Operand.ToString().Contains(nameof(ModLoader.ModLoader)));
        }

    }
}
