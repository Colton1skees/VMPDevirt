using Iced.Intel;
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
        Temporary,
        VirtualContexIndexOperand,
    }

    public abstract class ILOperand
    {
        public abstract OperandType Type { get; }

        public ImmediateOperand Immediate
        {
            get 
            {
                return (ImmediateOperand)this; 
            }
        }

        public RegisterOperand Register
        {
            get
            {
                return (RegisterOperand)this;
            }
        }

        public TemporaryOperand Temporary
        {
            get
            {
                return (TemporaryOperand)this;
            }
        }

        public VirtualContextIndexOperand VirtualContextIndex
        {
            get
            {
                return (VirtualContextIndexOperand)this;
            }
        }

        public bool IsImmediate()
        {
            return Type == OperandType.Immediate;
        }

        public bool IsReg()
        {
            return Type == OperandType.Register;
        }

        public bool IsTemporary()
        {
            return Type == OperandType.Temporary;
        }

        public bool IsVirtualContextIndex()
        {
            return Type == OperandType.VirtualContexIndexOperand;
        }

        public static ILOperand Create(ulong value)
        {
            return new ImmediateOperand(value);
        }

        public static ILOperand Create(long value)
        {
            return new ImmediateOperand(value);
        }

        public static ILOperand Create(Register register)
        {
            return new RegisterOperand(register);
        }

    }
}
