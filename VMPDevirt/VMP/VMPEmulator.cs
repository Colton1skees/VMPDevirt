using Dna.Core.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        private FunctionTracer tracer;

        private List<ulong> breakpoints = new List<ulong>();

        bool pendingEmulateBlock = false;

        public VMPEmulator(Devirtualizer _devirtualizer)
        {
            devirtualizer = _devirtualizer;
            tracer = new FunctionTracer(X86Mode.b64, devirtualizer.Dna);
            tracer.SetInstructionExecutionCallback(EmulateInstructionCallback);
        }

        private void EmulateInstructionCallback(Emulator _emulator, ulong address, int size, object userToken)
        {
            var emulator = (X86Emulator)_emulator;
            Console.WriteLine(devirtualizer.Dna.Disassembler.DisassembleBytesAtBinaryLocation(address));

            lock(breakpoints)
            {
                if (breakpoints.Contains(address))
                {
                    pendingEmulateBlock = true;
                    emulator.Stop();
                }
            }

        }

        public void Stop()
        {
            tracer.Emulator.Stop();
        }

        public void Resume()
        {
            tracer.Emulator.Start((ulong)tracer.Emulator.Registers.RIP, ulong.MaxValue);
        }

        public void EmulateUntil(List<ulong> pendingAddresses)
        {
            lock (breakpoints)
            {
                breakpoints.Clear();
                breakpoints.AddRange(pendingAddresses);
            }
        }

        public void EmulateUntil(ulong address)
        {
            EmulateUntil(new List<ulong>() { address });
        }

        public void TraceFunction(ulong address)
        {
            tracer.TraceFunction(address);
        }

    }
}
