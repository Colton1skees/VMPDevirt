using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class VirtualContextIndexOperand : ExprOperand
    {
        public override ExprOperandType Type => ExprOperandType.VirtualContextIndex;

        /// <summary>
        /// The index of the virtual context operand.
        /// </summary>
        public ulong Index { get; set; }

        public VirtualContextIndexOperand(ulong _index)
        {
            Index = _index;
        }

        public override string ToString()
        {
            return String.Format("vcr[0x{0}]", Index.ToString("X"));
        }

        public override int GetSize()
        {
            throw new NotImplementedException();
        }
    }
}
