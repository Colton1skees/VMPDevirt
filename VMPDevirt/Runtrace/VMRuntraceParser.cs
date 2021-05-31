using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.Runtrace
{
    public static class VMRuntraceParser
    {
        public static List<ulong> GetAllHandlerAddressesFromRuntrace(string path)
        {
            var lines = File.ReadAllLines(path);
            List<ulong> startAddresses = new List<ulong>();

            // Extract new-line delineated handler starts.
            bool isHandlerStart = false;
            foreach(var line in lines)
            {
                if (line.Contains(Environment.NewLine) || line.Length < 5)
                {
                    isHandlerStart = true;
                    continue;
                }

                if (isHandlerStart)
                {
                    //Console.WriteLine("handler start: " + line);
                    var split = line.Split(" -").First().Split(": ")[1];
                    startAddresses.Add(ulong.Parse(split, System.Globalization.NumberStyles.HexNumber));
                    isHandlerStart = false;
                }
            }

            return startAddresses;
        }
    }
}
