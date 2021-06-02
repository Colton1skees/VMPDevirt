using Dna.Core;
using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private int lastTemporaryID;

        public VMPLifter(Devirtualizer _devirt, VMPEmulator _emulator)
        {
            devirt = _devirt;
            vmpState = devirt.VMState;
            emulator = _emulator;
            lifterFunctions = new Dictionary<Action<List<ILInstruction>>, int>();
            lifterFunctions.Add(LiftVCPop, 5);
            lifterFunctions.Add(LiftLConst, 4);
            lifterFunctions.Add(LiftVCPush, 5);
            lifterFunctions.Add(LiftVADD, 6);
            lifterFunctions.Add(LiftPushVSP, 3);
            lifterFunctions.Add(LiftVNAND, 8);
        }

        public List<ILInstruction> LiftHandlerToIL(List<Instruction> instructions)
        {
            // Create a collection for the results of each lifter
            var possibleLifters = lifterFunctions.Where(x => x.Value == instructions.Count);
            Dictionary<int, List<ILInstruction>> liftedCollections = new Dictionary<int, List<ILInstruction>>();
            for(int i = 0; i < possibleLifters.Count(); i++)
            {
                liftedCollections.Add(i, new List<ILInstruction>());
            }

            // For each instruction, we pass it into a lifting function and hope that it was expecting this type of instruction
            for (instructionIndex = 0; instructionIndex < instructions.Count; instructionIndex++)
            {
                // Iterate through each lifter
                ins = instructions[instructionIndex];
                for (int lifterIndex = 0; lifterIndex < possibleLifters.Count(); lifterIndex++)
                {
                    // retrieve the collection for storing the output instructions, and execute the lifter if the collection is not null
                    var outputList = liftedCollections[lifterIndex];
                    if (!outputList.Any(x => x == null))
                    {
                        try
                        {
                            // attempt to lift the current instruction, and automatically store it in the output list
                            possibleLifters.ElementAt(lifterIndex).Key(outputList);
                        }

                        catch (Exception ex)
                        {
                            // add a null instruction to indicate that the handler failed to match
                            outputList.Add(null);
                        }
                    }
                    
                }

                if(instructionIndex < instructions.Count - 1)
                {
                    emulator.SingleStepUntil(instructions[instructionIndex + 1].IP);
                }

            }

            var matches = liftedCollections.Where(x => !x.Value.Any(y => y == null)).ToList();
            if(matches.Count() == 0)
            {
                // TODO: output handler instructions in exception
                throw new InvalidHandlerMatchException(String.Format("Failed to lift handler: no matches found."));
            }

            else if(matches.Count() > 1)
            {
                throw new InvalidHandlerMatchException(String.Format("Failed to lift handler: Found {0} matches when we expected 1.", matches.Count()));
            }

           return matches.Single().Value;
        }


        /*
            movzx eax,byte ptr [rsi] 
            add rsi,1 
            mov rcx,[rbp] 
            add rbp,8 
            mov [rsp+rax],rcx 
        */
        private void LiftVCPop(List<ILInstruction> outputInstructions)
        {
            switch(instructionIndex)
            {
                case 0:
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
                    ulong offset = GetVCPOffset();
                    outputInstructions.Add(new ILInstruction(ILOpcode.POP, new VirtualContextIndexOperand(offset)));
                    break;

                default:
                    throw new InvalidHandlerMatchException();
            }

        }

        /*
            mov rax,[rsi]
            add rsi,8
            sub rbp,8
            mov[rbp],rax
        */
        private void LiftLConst(List<ILInstruction> outputInstructions)
        {
            switch (instructionIndex)
            {
                case 0:
                    ReadVIP();
                    break;

                case 1:
                    ShiftVIP();
                    break;

                case 2:
                    ShiftVSP();
                    break;

                case 3:
                    WriteToVSP();
                    ulong immediate = emulator.ReadRegister(ins.Op1Register);
                    outputInstructions.Add(new ILInstruction(ILOpcode.PUSH, ILOperand.Create(immediate)));
                    break;

                default:
                    throw new InvalidHandlerMatchException();
            }

        }

        /*
            movzx eax,byte ptr [rsi]
            add rsi,1
            mov rax,[rsp+rax]
            sub rbp,8
            mov [rbp],rax
        */
        private void LiftVCPush(List<ILInstruction> outputInstructions)
        {
            switch (instructionIndex)
            {
                case 0:
                    ReadVIP();
                    break;

                case 1:
                    ShiftVIP();
                    break;

                case 2:
                    ReadVCP();
                    ulong offset = GetVCPOffset();
                    outputInstructions.Add(new ILInstruction(ILOpcode.PUSH, new VirtualContextIndexOperand(offset)));
                    break;

                case 3:
                    ShiftVSP();
                    break;

                case 4:
                    WriteToVSP();
                    break;

                default:
                    throw new InvalidHandlerMatchException();
            }

        }

        /*
            mov rax,[rbp]
            mov rcx,[rbp+8]
            add rax,rcx
            mov [rbp+8],rax
            pushfq
            pop qword ptr [rbp]
        */
        private void LiftVADD(List<ILInstruction> outputInstructions)
        {
            switch (instructionIndex)
            {
                case 0:
                    ReadVSP();
                    break;

                case 1:
                    ReadVSP();
                    break;

                case 2:
                    Expect(ins.Mnemonic == Mnemonic.Add && ins.Op0Kind == OpKind.Register && ins.Op1Kind == OpKind.Register);
                    var t0 = GetNewTemporary();
                    var t1 = GetNewTemporary();
                    outputInstructions.Add(new ILInstruction(ILOpcode.POP, t0));
                    outputInstructions.Add(new ILInstruction(ILOpcode.POP, t1));
                    outputInstructions.Add(new ILInstruction(ILOpcode.ADD, t0, t1));
                    outputInstructions.Add(new ILInstruction(ILOpcode.COMPUTEFLAGS));
                    outputInstructions.Add(new ILInstruction(ILOpcode.PUSH, t0));
                    outputInstructions.Add(new ILInstruction(ILOpcode.PUSHFLAGS));
                    break;

                case 3:
                    WriteToVSP();
                    break;

                case 4:
                    PushFlags();
                    break;

                case 5:
                    PopFlags();
                    break;

                default:
                    throw new InvalidHandlerMatchException();
            }

        }

        /*
            mov rax,rbp
            sub rbp,8
            mov [rbp],rax
        */
        private void LiftPushVSP(List<ILInstruction> outputInstructions)
        {
            switch (instructionIndex)
            {
                case 0:
                    // Expect mov vcr, vsp
                    bool isMovVSP = ins.Mnemonic == Mnemonic.Mov &&
                        ins.Op0Kind == OpKind.Register && ins.Op1Kind == OpKind.Register &&
                        ins.Op1Register == vmpState.VSP;
                    Expect(isMovVSP);
                    break;

                case 1:
                    ShiftVSP();
                    break;

                case 2:
                    WriteToVSP();
                    outputInstructions.Add(new ILInstruction(ILOpcode.PUSHVSP));
                    break;

                default:
                    throw new InvalidHandlerMatchException();
            }

        }

        /*
            mov rax,[rbp]
            mov rcx,[rbp+8]
            not rax
            not rcx
            and rax,rcx
            mov [rbp+8],rax
            pushfq
            pop qword ptr [rbp]
        */
        private void LiftVNAND(List<ILInstruction> outputInstructions)
        {
            switch (instructionIndex)
            {
                case 0:
                    ReadVSP();
                    break;

                case 1:
                    ReadVSP();
                    break;

                case 2:
                    Expect(ins.Mnemonic == Mnemonic.Not && ins.Op0Kind == OpKind.Register);
                    break;

                case 3:
                    Expect(ins.Mnemonic == Mnemonic.Not && ins.Op0Kind == OpKind.Register);
                    break;

                case 4:
                    Expect(ins.Mnemonic == Mnemonic.And && ins.Op0Kind == OpKind.Register && ins.Op1Kind == OpKind.Register);
                    var t0 = GetNewTemporary();
                    var t1 = GetNewTemporary();
                    outputInstructions.Add(new ILInstruction(ILOpcode.POP, t0));
                    outputInstructions.Add(new ILInstruction(ILOpcode.POP, t1));
                    outputInstructions.Add(new ILInstruction(ILOpcode.NAND, t0, t1));
                    outputInstructions.Add(new ILInstruction(ILOpcode.COMPUTEFLAGS));
                    outputInstructions.Add(new ILInstruction(ILOpcode.PUSH, t0));
                    outputInstructions.Add(new ILInstruction(ILOpcode.PUSHFLAGS));
                    break;

                case 5:
                    WriteToVSP();
                    break;


                case 6:
                    PushFlags();
                    break;

                case 7:
                    PopFlags();
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

        private void ReadVCP()
        {
            bool isReadVCP = (ins.Mnemonic == Mnemonic.Mov) &&
                ins.MemoryBase == vmpState.VCP && ins.MemoryIndex != Register.None &&
                ins.Op0Kind == OpKind.Register;

            Expect(isReadVCP);
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

        /// <summary>
        /// mov vsp, reg
        /// </summary>
        private void WriteToVSP()
        {
            bool isWriteToVSP = (ins.Mnemonic == Mnemonic.Mov) &&
                ins.MemoryBase == vmpState.VSP &&
                ins.Op1Kind == OpKind.Register;

            Expect(isWriteToVSP);
        }

        /// <summary>
        /// mov [VCP + offset], reg
        /// </summary>
        private void WriteToVCPOffset()
        {
            bool isWriteVCP = (ins.Mnemonic == Mnemonic.Mov) &&
                ins.MemoryBase == vmpState.VCP && ins.MemoryIndex != Register.None &&
                ins.Op1Kind == OpKind.Register;

            Expect(isWriteVCP);
        }

        /// <summary>
        /// pushfq
        /// </summary>
        private void PushFlags()
        {
            bool isPushFlags = ins.Mnemonic == Mnemonic.Pushfq;
            Expect(isPushFlags);
        }

        private void PopFlags()
        {
            bool isStoreFlags = ins.Mnemonic == Mnemonic.Pop &&
                ins.MemoryBase == vmpState.VSP;

            Expect(isStoreFlags);
        }

        private ulong GetVCPOffset()
        {
            return emulator.ReadRegister(ins.MemoryIndex);
        }

        private ulong GetOp0RegisterValue()
        {
            return emulator.ReadRegister(ins.Op0Register);
        }

        private ulong GetOp1RegisterValue()
        {
            return emulator.ReadRegister(ins.Op1Register);
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

        /// <summary>
        /// Allocates an ID for a new temporary
        /// </summary>
        /// <returns></returns>
        private TemporaryOperand GetNewTemporary()
        {
            int id = lastTemporaryID;
            lastTemporaryID++;

            return new TemporaryOperand(id);
        }
    }
}
