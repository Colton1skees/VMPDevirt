using System;
using System.Collections.Generic;
using System.Text;
using VMPDevirt.VMP.ILExpr.Operands;

namespace VMPDevirt.VMP.ILExpr
{
    public class StackExpression : ILExpression
    {
        public override ExprType Type => ExprType.Stack;

        public StackExpression(ExprOpCode _opCode, ExprOperand opOne) : base(_opCode, opOne)
        {
            OpCode = _opCode;
        }
    }
}
