using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class ImmediateOperand : ExprOperand
    {
        private ulong immValue;

        private int size;

        public override ExprOperandType Type => ExprOperandType.Immediate;

        /// <summary>
        /// The unsigned representation of the immediate operand.
        /// </summary>
        public ulong U64
        {
            get { return immValue; }
            private set { immValue = value; }
        }

        /// <summary>
        /// The signed representation of the immediate operand.
        /// </summary>
        public long I64
        {
            get { return (long)immValue; }
            private set { immValue = (ulong)value; }
        }

        public ImmediateOperand(ulong _U64, int _size)
        {
            U64 = _U64;
            size = _size;
        }

        public ImmediateOperand(long _I64, int _size)
        {
            I64 = _I64;
            size = _size;
        }

        public override int GetSize()
        {
            return size;
        }

        public override string ToString()
        {
            return "0x" + immValue.ToString("X");
        }
    }
}
