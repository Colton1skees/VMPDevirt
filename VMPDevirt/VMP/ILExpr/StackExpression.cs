﻿using System;
using System.Collections.Generic;
using System.Text;
using VMPDevirt.VMP.ILExpr.Operands;

namespace VMPDevirt.VMP.ILExpr
{
    public class StackExpression : ILExpression
    {
        public override ExprType Type => ExprType.Stack;

        public StackExpression(ExprOpCode _opCode, ExprOperand _lhs) : base(_opCode, _lhs)
        {
            OpCode = _opCode;
        }
    }
}
