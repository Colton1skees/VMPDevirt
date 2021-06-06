using System;
using System.Collections.Generic;
using System.Text;
using VMPDevirt.VMP.ILExpr.Operands;

namespace VMPDevirt.VMP.ILExpr
{
    public class SpecialExpression : ILExpression
    {
        public SpecialExpression(ExprOpCode _opCode, ExprOperand _opOne = null, ExprOperand _opTwo = null, ExprOperand _opThree = null) : base(_opCode, _opOne, _opTwo, _opThree)
        {

        }

        public override ExprType Type => ExprType.Special;
    }
}
