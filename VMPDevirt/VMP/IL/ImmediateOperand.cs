using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP.IL
{
    public class ImmediateOperand : ILOperand
    {
        private ulong _value;

        /// <summary>
        /// The type of the operand(e.g immediate or register).
        /// </summary>
        public override OperandType Type { get; } = OperandType.Immediate;

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
    }
}
