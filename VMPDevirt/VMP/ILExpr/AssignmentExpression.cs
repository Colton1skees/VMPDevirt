using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VMPDevirt.VMP.ILExpr.Operands;

namespace VMPDevirt.VMP.ILExpr
{
    public class AssignmentExpression : ILExpression
    {
        public override ExprType Type => ExprType.Assignment;

        public ExprOperand DestinationOperand { get; set; }

        public AssignmentExpression(ExprOpCode _opCode, ExprOperand _destination, ExprOperand _opOne = null, ExprOperand _opTwo = null, ExprOperand _opThree = null) : base(_opCode, _opOne, _opTwo, _opThree)
        {
            DestinationOperand = _destination;
            if(!IsValidDestination())
            {
                throw new Exception(String.Format("Failed to create AssignmentExpression. The operand {0} is not a valid destination operand.", DestinationOperand));
            }
        }

        /// <summary>
        /// Validates whether or not the provided destination operand is a valid operator(e.g we can't write to immediates).
        /// </summary>
        /// <returns></returns>
        private bool IsValidDestination()
        {
            if (DestinationOperand == null)
                return false;

            var type = DestinationOperand.Type;
            return type != ExprOperandType.Immediate;
        }

        public override int GetSize()
        {
            return DestinationOperand.GetSize();
        }

        public override string ToString()
        {
            int op = OpCount;
            string result = null;

            if (op == 1)
                result = String.Format("{0} = {1} {2}", DestinationOperand, GetOpCodeWithSize(), Op1);
            else if (op == 2)
                result = String.Format("{0} = {1} {2}, {3}", DestinationOperand, GetOpCodeWithSize(), Op1, Op2);
            else if(op == 3)
                result = String.Format("{0} = {1} {2}, {3}, {4}", DestinationOperand, GetOpCodeWithSize(), Op1, Op2, Op3);
            else
                throw new Exception(String.Format("Failed to convert assignment expression to string. The operand count {0} is not valid.", op));

            return result.ToLower();
        }
    }
}
