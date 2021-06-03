using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class SpecialExpression : ILExpression
    {
        public SpecialExpression(ExprOpCode _opCode, ExprOperand _lhs = null, ExprOperand _rhs = null)
        {
            OpCode = _opCode;
            LHS = _lhs;
            RHS = _rhs;
        }

        public override ExprType Type => ExprType.Special;

        public override int GetSize()
        {
            throw new NotImplementedException();
        }
    }
}
