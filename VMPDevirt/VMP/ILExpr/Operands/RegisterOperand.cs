using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class RegisterOperand : ExprOperand
    {
        public override ExprOperandType Type => ExprOperandType.Register;

        public Register Reg { get; set; }

        public RegisterOperand(Register _reg)
        {
            Reg = _reg;
        }

        public static implicit operator RegisterOperand(Register _reg)
        {
            return new RegisterOperand(_reg);
        }

        public override string ToString()
        {
            return Reg.ToString();
        }

        public override int GetSize()
        {
            throw new NotImplementedException();
        }
    }
}
