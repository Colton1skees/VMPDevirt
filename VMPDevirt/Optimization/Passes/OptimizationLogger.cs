using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.Optimization.Passes
{
    public class OptimizationLogger
    {
        public static void LogInfo(string text, object[] args = null)
        {
            Log("Info: " + text, args);
        }

        public static void LogWeirdBehavior(string text, object[] args = null)
        {
            Log("WeirdBehavior: " + text, args);
        }

        private static void Log(string text, object[] args)
        {
            Console.WriteLine(text, args);
        }
    }
}
