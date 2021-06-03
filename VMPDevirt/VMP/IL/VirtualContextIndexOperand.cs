using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.IL
{
    public class VirtualContextIndexOperand : ILOperand
    {
        public override OperandType Type { get; } = OperandType.Immediate;

        /// <summary>
        /// The index of the virtual context operand.
        /// </summary>
        public ulong Index { get; set; }

        public VirtualContextIndexOperand(ulong _index, int size)
        {
            Index = _index;
        }

        public override string ToString()
        {
            return String.Format("vcr[0x{0}]", Index.ToString("X"));
        }
    }
}
