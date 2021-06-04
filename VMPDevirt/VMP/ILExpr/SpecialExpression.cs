using System;
using System.Collections.Generic;
using System.Text;
using VMPDevirt.VMP.ILExpr.Operands;

namespace VMPDevirt.VMP.ILExpr
{
    public class SpecialExpression : ILExpression
    {
        public SpecialExpression(ExprOpCode _opCode, ExprOperand _lhs = null, ExprOperand _rhs = null) : base(_opCode, _lhs, _rhs)
        {

        }

        public override ExprType Type => ExprType.Special;
    }
}
