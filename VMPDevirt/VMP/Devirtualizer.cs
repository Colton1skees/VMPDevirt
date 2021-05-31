using Iced.Intel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMPDevirt.Runtrace;

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
            DumpHandlers();
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

        
    }
}
