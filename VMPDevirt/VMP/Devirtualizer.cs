using Iced.Intel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMPDevirt.Runtrace;
using VMPDevirt.VMP.IL;

namespace VMPDevirt.VMP
{
    public class Devirtualizer
    {
        public Dna.Dna Dna { get; }

        public VMPState VMState { get; }

        private HandlerOptimizer handlerOptimizer;


        private readonly string runtracePath;

        public Devirtualizer(Dna.Dna _dna, string _runtracePath)
        {
            Dna = _dna;
            runtracePath = _runtracePath;
            handlerOptimizer = new HandlerOptimizer(this);
            VMState = new VMPState(Register.RBP, Register.RSI, Register.RSP, Register.RBX, Register.RAX);
        }

        public void Execute()
        {
            // DumpHandlers();

            // Initialize the emulator and single step until we reach the first handler
            VMPEmulator emulator = new VMPEmulator(this);

            // Start emulating the virtualized function. Note: The emulator actually only implements single step functionality, so this only starts executing the first instruction.
            ulong vmEntry = 0x1400FD439;
            emulator.TraceFunction(vmEntry);
            VMPLifter lifter = new VMPLifter(this, emulator);

            List<ILInstruction> ilInstructions = new List<ILInstruction>();
            while(true)
            {

                var rip = emulator.ReadRegister(Register.RIP);
                var instruction = Dna.Disassembler.DisassembleBytesAtBinaryLocation(rip);

                if (IsDispatcherInstruction(instruction))
                {
                    emulator.SingleStep();
                    var handlerGraph = Dna.FunctionParser.GetControlFlowGraph(emulator.ReadRegister(Register.RIP));
                    var handlerInstructions = handlerOptimizer.OptimizeHandler(handlerGraph.GetInstructions().ToList());


                    var liftedInstructions = lifter.LiftHandlerToIL(handlerInstructions);
                    foreach(var liftedInstruction in liftedInstructions)
                    {
                        liftedInstruction.Address = handlerInstructions.First().IP;
                    }
                    ilInstructions.AddRange(liftedInstructions);


                    foreach(var ilInstruction in ilInstructions)
                    {
                        Console.WriteLine("ILInstruction({0}): {1}", ilInstruction.Address.ToString("X"), ilInstruction);
                    }
                }

                else
                {
                    emulator.SingleStep();
                }
            }
        }

        public void DumpHandlers()
        {
            // Extract all unique handlers
            var handlerAddresses = VMRuntraceParser.GetAllHandlerAddressesFromRuntrace(runtracePath);
            Dictionary<string, ulong> handlerTextCollection = new Dictionary<string, ulong>();
            foreach (var handlerAddr in handlerAddresses)
            {
                // temporary hack to avoid attempting to parse an invalid location
                if (handlerAddr == 0x1400FD443)
                    continue;

                // Parse the handler, optimize it, and store the output if it is unique
                var graph = Dna.FunctionParser.GetControlFlowGraph(handlerAddr);
                var handlerInstructions = handlerOptimizer.OptimizeHandler(graph.GetInstructions().ToList());
                var handlerText = String.Join("\n", handlerInstructions.Select(x => x.ToString()));

                if (!handlerTextCollection.ContainsKey(handlerText))
                    handlerTextCollection.Add(handlerText, handlerAddr);
            }

            // Dump all unique handlers to file
            List<string> outputLines = new List<string>();
            int i = 0;
            foreach(var handler in handlerTextCollection)
            {
                outputLines.Add(handler.Value.ToString("X"));
                outputLines.Add(handler.Key);
                outputLines.Add(Environment.NewLine);
            }

            File.WriteAllLines("../../../optimized_handlers.txt", outputLines);
        }

        private bool IsDispatcherInstruction(Instruction instruction)
        {
            if (instruction.Mnemonic == Mnemonic.Ret || (instruction.Mnemonic == Mnemonic.Jmp && instruction.Op0Kind == OpKind.Register))
                return true;

            return false;
        }

        
    }
}
