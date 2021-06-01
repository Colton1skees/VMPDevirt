using Dna.Core.Binary.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using VMPDevirt.VMP;

namespace VMPDevirt
{
    class Program
    {

        static void Devirt()
        {
            // Load binary into analysis framework
            string binaryPath = @"C:\Users\colton\Desktop\Reversing\IDBs\Vmp\T4USample\devirtualizeme64_vmp_3.0.9_v1.exe";
            WindowsBinary binary = new WindowsBinary(64, File.ReadAllBytes(binaryPath), true, 0x140000000);
            Dna.Dna dna = new Dna.Dna(binary);

            // Execute
            Devirtualizer devirt = new Devirtualizer(dna, @"C:\Users\colton\Desktop\Reversing\IDBs\Vmp\T4USample\split_runtrace.txt");
            devirt.Execute();

            /*
            VMPEmulator emulator = new VMPEmulator(devirt);

            ulong addrDispatcher = 0x1400F818A;
            emulator.EmulateUntil(addrDispatcher);

            ulong addrVMStart = 0x1400FD439;
            emulator.TraceFunction(addrVMStart);


            Console.WriteLine("HIT EMULATED POINT");
            */
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            Devirt();
            Console.WriteLine("Finished... press enter to exit...");
            Console.ReadLine();
        }
    }
}
