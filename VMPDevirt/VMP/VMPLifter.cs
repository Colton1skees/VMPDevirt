using Dna.Core;
using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMPDevirt.VMP.IL;

namespace VMPDevirt.VMP
{
    /// <summary>
    /// Provides functionality for lifting a handler
    /// </summary>
    public class VMPLifter
    {
        private readonly Devirtualizer devirt;

        private VMPEmulator emulator;

        private VMPState vmpState;

        private Instruction ins;

        private int instructionIndex;

        private Dictionary<Action<List<ILInstruction>>, int> lifterFunctions;

        public VMPLifter(Devirtualizer _devirt)
        {
            devirt = _devirt;
            vmpState = devirt.VMState;
            lifterFunctions = new Dictionary<Action<List<ILInstruction>>, int>();
            lifterFunctions.Add(LiftVCPop, 5);
 
        }

        public List<ILInstruction> LiftHandlerToIL(List<Instruction> instructions, VMPEmulator emulator)
        {
            List<ILInstruction> finalLiftedInstructions = null;

            // Create a collection for the results of each lifter
            Dictionary<int, List<ILInstruction>> liftedCollections = new Dictionary<int, List<ILInstruction>>();
            for(int i = 0; i < lifterFunctions.Count; i++)
            {
                liftedCollections.Add(i, new List<ILInstruction>());
            }

            // For each instruction, we pass it into a lifting function and hope that it was expecting this type of instruction
            for (instructionIndex = 0; instructionIndex < instructions.Count; instructionIndex++)
            {
                var instruction = instructions[instructionIndex];
                for(int lifterIndex = 0; lifterIndex < lifterFunctions.Count; lifterIndex++)
                {
                    // retrieve the collection for storing the output instructions
                    var outputList = liftedCollections[lifterIndex];

                    try
                    {
                        // attempt to lift the current instruction, and automatically store it in the output list
                        lifterFunctions[lifterIndex](outputList);
                    }

                    catch(Exception ex)
                    {
                        // clear the output list if we fail to lift a single instruction
                        outputList.Clear();
                    }
                    
                }
            }

            // If we have lifted every instruction, then we try to locate the only matching handler by lifting an invalid instruction and catching the exception
            // For now we have to do this, since I didn't feel like hardcoding the length o

            return null;
        }

        private void LiftVCPop(List<ILInstruction> outputInstructions)
        {
            switch(instructionIndex)
            {
                case 0:
                    outputInstructions.Add(new ILInstruction(ILOpcode.VCPOP, ILOperand.Create(10), ILOperand.Create(10)));
                    ReadVIP();
                    break;

                case 1:
                    ShiftVIP();
                    break;

                case 2:
                    ReadVSP();
                    break;

                case 3:
                    ShiftVSP();
                    break;

                case 4:
                    WriteToVCPOffset();
                    break;

                default:
                    throw new InvalidHandlerMatchException();
            }

        }

        private void LiftNonExistent(List<ILInstruction> outputInstructions)
        {
            switch (instructionIndex)
            {
                case 0:
                    ShiftVSP();
                    break;

                case 1:
                    ReadVIP();
                    break;

                case 2:
                    ReadVIP();
                    break;

                case 3:
                    ReadVIP();
                    break;

                case 4:
                    ReadVIP();
                    break;

                default:
                    throw new InvalidHandlerMatchException();
            }

        }

        /// <summary>
        /// mov reg, [vip]
        /// </summary>
        private void ReadVIP()
        {
            bool isReadVIP = (ins.Mnemonic == Mnemonic.Mov || ins.Mnemonic == Mnemonic.Movzx) &&
                ins.Op0Kind == OpKind.Register && ins.MemoryBase == vmpState.VIP;

            Expect(isReadVIP);
        }

        /// <summary>
        /// mov reg, [vsp]
        /// </summary>
        private void ReadVSP()
        {
            bool isReadVSP = ins.Mnemonic == Mnemonic.Mov || ins.Mnemonic == Mnemonic.Movzx &&
                ins.Op0Kind == OpKind.Register && ins.MemoryBase == vmpState.VSP;

            Expect(isReadVSP);
        }

        /// <summary>
        /// (add || sub) vip, immediate
        /// </summary>
        private void ShiftVIP()
        {
            bool isShiftVIP = (ins.Mnemonic == Mnemonic.Add || ins.Mnemonic == Mnemonic.Sub) &&
                ins.Op0Register == vmpState.VIP &&
                ins.Op1Kind.IsImmediate();

            Expect(isShiftVIP);
        }

        /// <summary>
        /// (add || sub) vsp, immediate)
        /// </summary>
        private void ShiftVSP()
        {
            bool isShiftVSP = (ins.Mnemonic == Mnemonic.Add || ins.Mnemonic == Mnemonic.Sub) &&
                ins.Op0Register == vmpState.VSP &&
                ins.Op1Kind.IsImmediate();

            Expect(isShiftVSP);
        }

        private void WriteToVCPOffset()
        {
            bool isWriteVCP = (ins.Mnemonic == Mnemonic.Mov) &&
                ins.MemoryBase == vmpState.VCP && ins.MemoryIndex != Register.None;

            Expect(isWriteVCP);
        }


        /*
        private bool Expect(Mnemonic mnemonic, object operandOne = null, object operandTwo = null)
        {
            return ins.Is(mnemonic, operandOne, operandTwo);
        }
        */

        private void Expect(bool input)
        {
            if(!input)
                throw new Exception("Error matching handler. Failed to match instruction");
        }
    }
}
