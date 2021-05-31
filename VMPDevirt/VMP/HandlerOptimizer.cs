using Dna.Core;
using Dna.Core.ControlFlow;
using Dna.Core.ControlFlow.Parser;
using Dna.Core.Optimization;
using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP
{
    public class HandlerOptimizer
    {
        private Devirtualizer devirtualizer;

        private BasicBlock handlerBlock;


        public HandlerOptimizer(Devirtualizer _devirtualizer)
        {
            devirtualizer = _devirtualizer;
        }

        /// <summary>
        /// Optimizes a VM handler down to a pattern matchable state.
        /// </summary>
        /// <param name="handlerInstructions"></param>
        /// <returns></returns>
        public List<Instruction> OptimizeHandler(List<Instruction> handlerInstructions)
        {
            // Initialize a single valid basic block for the handler.
            handlerBlock = new BasicBlock();
            handlerBlock.Address = handlerInstructions.First().IP;
            handlerBlock.Instructions.AddRange(handlerInstructions);

            // Optimize away everything except for the core handler instructions.
            RemoveBranches();
            RemoveDispatcher();
            RemoveDeadcode();
            RemoveRollingEncryption();
            RemoveDeadcode();
            return handlerBlock.Instructions;
        }

        /// <summary>
        /// Removes all branching instructions from the current handler.
        /// </summary>
        private void RemoveBranches()
        {
            // VMProtect handlers do not have any true branching, other than the virtual stack expansion, which we want to filter out when optimizing the VM handler.
            this.handlerBlock.Instructions.RemoveAll(x => x.FlowControl.IsBranch());
        }

        /// <summary>
        /// Removes all dispatching specific instructions from the handler.
        /// </summary>
        private void RemoveDispatcher()
        {
            var dispatcherStartIndex = handlerBlock.Instructions.FindLastIndex(x => x.ToString() == "mov eax,[rsi]");
            if (dispatcherStartIndex != -1)
                handlerBlock.Instructions.RemoveRange(dispatcherStartIndex, handlerBlock.Instructions.Count - dispatcherStartIndex);
            else
                Console.WriteLine("Failed to remove dispatcher from function");
        }

        private void RemoveDeadcode()
        {
            // Enhance the deadcode elimination pass by pinning the internal VMState registers and forcibly discarding volatile registers.
            var assembler = new Assembler(devirtualizer.Dna.Binary.Bitness);
            var registers = Helpers.Get64BitRegisters();
            foreach(var reg in registers)
            {
                // If a register is volatile, then assemble instructions to discard the value after the execution of the VM handler. 
                if (reg != devirtualizer.VMState.VCP && reg != devirtualizer.VMState.VIP && reg != devirtualizer.VMState.VSP && reg != devirtualizer.VMState.VRK)
                {
                    assembler.mov(reg.GetReg64(), 0);
                }
            }

            // Assemble an instruction to discard the eflags.
            assembler.test(Register.RSP.GetReg64(), Register.RSP.GetReg64());

            // Append the assemble instructions to the end of the handler
            var instructionsToAppend = assembler.Instructions.ToList();
            var firstAppendedInstruction = instructionsToAppend.First();
            handlerBlock.Instructions.AddRange(instructionsToAppend);

            // Apply the deadcode elimination pass until we reach a point where no new deadcode can be found
            int lastInstructionCount = handlerBlock.Instructions.Count;
            while(true)
            {
                new PassDeadcodeElimination(devirtualizer.Dna).RemoveBlockMutations(handlerBlock);
                if (lastInstructionCount == handlerBlock.Instructions.Count)
                    break;

                lastInstructionCount = handlerBlock.Instructions.Count;
            }

            // Remove the appended instructions for pinning/DCE purposes
            var firstAppendedInstructionIndex = handlerBlock.Instructions.IndexOf(firstAppendedInstruction);
            handlerBlock.Instructions.RemoveRange(firstAppendedInstructionIndex, handlerBlock.Instructions.Count - firstAppendedInstructionIndex);
        }

        /// <summary>
        /// Attempts to remove rolling encryption specific instructions based off of patterns.
        /// </summary>
        private void RemoveRollingEncryption()
        {
            /* Pattern One:
             * XOR VCR, VRK
             * {more transformations}
             * XOR VRK, VCR
             */ 

            var vrkStarts = handlerBlock.Instructions.Where(x => 
            x.Mnemonic == Mnemonic.Xor &&
            x.Op0Register.GetFullRegister() == devirtualizer.VMState.VCR && 
            x.Op1Register.GetFullRegister() == devirtualizer.VMState.VRK).ToList();

            var vrkEnds = handlerBlock.Instructions.Where(x => 
            x.Mnemonic == Mnemonic.Xor && 
            x.Op0Register.GetFullRegister() == devirtualizer.VMState.VRK && 
            x.Op1Register.GetFullRegister() == devirtualizer.VMState.VCR).ToList();

            if(vrkStarts.Count() == 1 && vrkEnds.Count() == 1)
            {
                var start = handlerBlock.Instructions.IndexOf(vrkStarts.Single());
                var end = handlerBlock.Instructions.IndexOf(vrkEnds.Single());
                handlerBlock.Instructions.RemoveRange(start, end - start + 1);
            }
        }


    }
}
