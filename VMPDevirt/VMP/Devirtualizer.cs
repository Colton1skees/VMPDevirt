using Iced.Intel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMPDevirt.Optimization.Passes;
using VMPDevirt.Runtrace;
using VMPDevirt.VMP.ILExpr;
using VMPDevirt.VMP.Routine;

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

            ILRoutine routine = new ILRoutine(0);
            var block = routine.AllocateEmptyBlock(0);
            while(true)
            {

                var rip = emulator.ReadRegister(Register.RIP);
                var instruction = Dna.Disassembler.DisassembleBytesAtBinaryLocation(rip);

                if (IsDispatcherInstruction(instruction))
                {
                    // Single step into the handler, parse the CFG, and optimize it to a pattern matchable state.
                    emulator.SingleStep();
                    var handlerGraph = Dna.FunctionParser.GetControlFlowGraph(emulator.ReadRegister(Register.RIP));
                    var nativeHandlerInstructions = handlerOptimizer.OptimizeHandler(handlerGraph.GetInstructions().ToList());

                    // Lift the instructions and append them to the routine's only basic block.
                    var liftedExpressions = lifter.LiftHandlerToIL(nativeHandlerInstructions);
                    block.Expressions.AddRange(liftedExpressions);
                    foreach (var expression in liftedExpressions)
                    {
                        expression.Address = nativeHandlerInstructions.First().IP;
                    }


                    // Log expressions to console for quick error diagnosing...
                    foreach(var expression in block.Expressions)
                    {
                        Console.WriteLine("ILInstruction({0}): {1}", expression.Address.ToString("X"), expression);
                    }

                    // Pass the lifted function into various optimization passes
                    if(liftedExpressions.Any(x => x.OpCode == ExprOpCode.VMEXIT))
                    {
                        Console.WriteLine("Finished lifting up until vexit...");

                        Console.WriteLine("PRE-OPTIMIZATION: ");
                        foreach (var expression in block.Expressions)
                        {
                            Console.WriteLine("ILInstruction({0}): {1}", expression.Address.ToString("X"), expression);
                        }
                        PassStackToAssignment pass = new PassStackToAssignment();
                        pass.Execute(block);
                        Console.WriteLine();
                        Console.WriteLine("POST-OPTIMIZATION: ");
                        foreach (var expression in block.Expressions)
                        {
                            Console.WriteLine("ILInstruction({0}): {1}", expression.Address.ToString("X"), expression);
                        }

                        Console.ReadLine();

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
