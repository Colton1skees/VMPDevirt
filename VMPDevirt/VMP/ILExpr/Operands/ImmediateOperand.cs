using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class ImmediateOperand : ExprOperand
    {
        private ulong _value;

        public override ExprOperandType Type => ExprOperandType.Immediate;

        /// <summary>
        /// The unsigned representation of the immediate operand.
        /// </summary>
        public ulong U64
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The signed representation of the immediate operand.
        /// </summary>
        public long I64
        {
            get { return (long)_value; }
            set { _value = (ulong)value; }
        }

        public ImmediateOperand(ulong _U64)
        {
            U64 = _U64;
        }

        public ImmediateOperand(long _I64)
        {
            I64 = _I64;
        }

        public static implicit operator ImmediateOperand(ulong _U64)
        {
            return new ImmediateOperand(_U64);
        }

        public static implicit operator ImmediateOperand(long _I64)
        {
            return new ImmediateOperand(_I64);
        }

        public override string ToString()
        {
            return "0x" + _value.ToString("X");
        }

        public override int GetSize()
        {
            throw new NotImplementedException();
        }
    }
}
