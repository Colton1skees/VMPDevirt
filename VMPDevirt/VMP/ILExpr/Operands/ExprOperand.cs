using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public enum ExprOperandType
    {
        Immediate,
        Register,
        Temporary,
        VirtualContextIndex,
    }

    public abstract class ExprOperand
    {
        public abstract ExprOperandType Type { get; }

        public abstract int GetSize();

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
            return Type == ExprOperandType.Immediate;
        }

        public bool IsReg()
        {
            return Type == ExprOperandType.Register;
        }

        public bool IsTemporary()
        {
            return Type == ExprOperandType.Temporary;
        }

        public bool IsVirtualContextIndex()
        {
            return Type == ExprOperandType.VirtualContextIndex;
        }

        public static ExprOperand Create(ulong value)
        {
            return new ImmediateOperand(value);
        }

        public static ExprOperand Create(long value)
        {
            return new ImmediateOperand(value);
        }

        public static ExprOperand Create(Register register)
        {
            return new RegisterOperand(register);
        }
    }
}
