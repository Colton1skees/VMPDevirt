using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP.IL
{
    public enum OperandType
    {
        Nil,
        Immediate,
        Register,
    }

    public abstract class ILOperand
    {
        public abstract OperandType Type { get; }

        public NilOperand Nil
        {
            get { return (NilOperand)this; }
        }

        public ImmediateOperand Immediate
        {
            get { return (ImmediateOperand)this; }
        }

        public RegisterOperand Register
        {
            get { return (RegisterOperand)this; }
        }

        public bool IsNil()
        {
            return Type == OperandType.Nil;
        }

        public bool IsImmediate()
        {
            return Type == OperandType.Immediate;
        }

        public bool IsReg()
        {
            return Type == OperandType.Register;
        }

    }
}
