using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public enum VirtualRegister
    {
        RFLAGS,
        VSP,
    }

    /// <summary>
    /// Represents any construct which cannot be represented under normal circumstances(e.g special register like VSP, and sadly eflags).
    /// </summary>
    public class VirtualRegisterOperand : ExprOperand
    {
        public override ExprOperandType Type => ExprOperandType.VirtualRegisterOperand;

        public VirtualRegister VirtualRegister { get; set; }

        public VirtualRegisterOperand(VirtualRegister _register)
        {
            VirtualRegister = _register;
        }

        public override int GetSize()
        {
            return VirtualRegister.GetSizeInBits();
        }

        public override string ToString()
        {
            return VirtualRegister.ToString();
        }

    }
}
