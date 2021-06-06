using Dna.Core;
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

        public virtual bool SupportsSSA => true;

        public int ValueNumber { get; set; }

        public ImmediateOperand Immediate => (ImmediateOperand)this;

        public TemporaryOperand Temporary => (TemporaryOperand)this;

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

        public static ImmediateOperand CreateImmediate(ulong value, int size)
        {
            return new ImmediateOperand(value, size);
        }

        public static ImmediateOperand CreateImmediate(long value, int size)
        {
            return new ImmediateOperand(value, size);
        }

        public static VirtualRegisterOperand CreateVirtualRegister(string name, int size)
        {
            return new VirtualRegisterOperand(name, size);
        }

        public static VirtualRegisterOperand CreateVirtualRegister(Register register)
        {
            if (register.GetSizeInBits() != 64)
                throw new Exception("TODO: Implement support for indexing lower ranges of registers... ");

            return new VirtualRegisterOperand(register.GetFullRegister().ToString(), 64);
        }

        public static VirtualRegisterOperand CreateVirtualRegisterForFlags()
        {
            return new VirtualRegisterOperand("rflags", 64);
        }
    }
}
