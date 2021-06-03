using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class VirtualContextIndexOperand : ExprOperand
    {
        public override ExprOperandType Type => ExprOperandType.VirtualContextIndex;

        private int size;

        /// <summary>
        /// The index of the virtual context operand.
        /// </summary>
        public ulong Index { get; set; }

        public VirtualContextIndexOperand(ulong _index, int _size)
        {
            if (_size == 0)
                Debugger.Break();
            Index = _index;
            size = _size;
        }

        public override string ToString()
        {
            return String.Format("vcr[0x{0}]", Index.ToString("X"));
        }

        public override int GetSize()
        {
            return size;
        }
    }
}
