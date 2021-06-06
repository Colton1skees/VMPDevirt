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
        VirtualRegisterOperand,
    }

    public abstract class ExprOperand
    {
        public abstract ExprOperandType Type { get; }

        public int Size => GetSize();

        public abstract int GetSize();

        public ImmediateOperand Immediate => (ImmediateOperand)this;

        public RegisterOperand Register => (RegisterOperand) this;

        public TemporaryOperand Temporary => (TemporaryOperand)this;

        public VirtualContextIndexOperand VirtualContextIndex => (VirtualContextIndexOperand)this;

        public VirtualRegisterOperand VirtualRegister => (VirtualRegisterOperand)this;

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

        public bool IsVirtualRegister()
        {
            return Type == ExprOperandType.VirtualRegisterOperand;
        }

        public static ExprOperand Create(ulong value, int size)
        {
            return new ImmediateOperand(value, size);
        }

        public static ExprOperand Create(long value, int size)
        {
            return new ImmediateOperand(value, size);
        }

        public static ExprOperand Create(Register register)
        {
            return new RegisterOperand(register);
        }
    }
}
