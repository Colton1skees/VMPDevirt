using Dna.Core.Tracing;
using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unicorn;
using Unicorn.X86;

namespace VMPDevirt.VMP
{
    /// <summary>
    /// Provides functionality for emulating VMP natively.
    /// </summary>
    public class VMPEmulator
    {
        private Devirtualizer devirtualizer;

        public FunctionTracer tracer;

        bool blockUntilSingleStep = true;


        public VMPEmulator(Devirtualizer _devirtualizer)
        {
            devirtualizer = _devirtualizer;
            tracer = new FunctionTracer(X86Mode.b64, devirtualizer.Dna);
            tracer.SetInstructionExecutionCallback(EmulateInstructionCallback);
        }

        private void EmulateInstructionCallback(Emulator _emulator, ulong address, int size, object userToken)
        {
            var emulator = (X86Emulator)_emulator;
            Console.WriteLine("Pre execution callback for: {0} {1}", address.ToString("X"), devirtualizer.Dna.Disassembler.DisassembleBytesAtBinaryLocation(address));

            // Block until we single step
            while(blockUntilSingleStep)
            {

            }

            blockUntilSingleStep = true;
                
        }

        public void SingleStep()
        {
            // TODO: FIX THREADING ISSUES
            Thread.Sleep(1);
            blockUntilSingleStep = false;
            while(blockUntilSingleStep == false)
            {

            }

           Thread.Sleep(2);
        }

        public void SingleStepUntil(ulong address)
        {
            while (ReadRegister(Register.RIP) != address)
                SingleStep();
        }

        public void RunUntil(ulong address)
        {
            while(tracer.Emulator.Registers.RIP != (long)address)
            {
                SingleStep();
            }

            Thread.Sleep(1);
        }

        public void TraceFunction(ulong address)
        {
            // Start tracing in a new thread
            new Thread(() =>
            {
                tracer.TraceFunction(address);
            }).Start();

            Thread.Sleep(150);
        }

        public ulong ReadRegister(Register register)
        {
            // Unclean hack, but it saves time :)
            return (ulong)(long)tracer.Emulator.Registers.GetType().GetProperty(register.ToString().ToUpper()).GetValue(tracer.Emulator.Registers, null);
        }

    }
}
