using System;
using System.Collections.Generic;
using System.Text;
using VMPDevirt.VMP.ILExpr.Operands;

namespace VMPDevirt.VMP.ILExpr
{
    public class AssignmentExpression : ILExpression
    {
        public override ExprType Type => ExprType.Assignment;

        public ExprOperand DestinationOperand { get; set; }

        public AssignmentExpression(ExprOpCode _opCode, ExprOperand _destination, ExprOperand _lhs = null, ExprOperand _rhs = null)
        {
            OpCode = _opCode;
            DestinationOperand = _destination;
            LHS = _lhs;
            RHS = _rhs;

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
            return type == ExprOperandType.Register || type == ExprOperandType.Temporary || type == ExprOperandType.VirtualContextIndex;
        }

        public override string ToString()
        {
            int op = OpCount;
            string result = null;

            if (op == 1)
                result = String.Format("{0} = {1} {2}", DestinationOperand, GetOpCodeWithSize(), LHS);
            else if (op == 2)
                result = String.Format("{0} = {1} {2}, {3}", DestinationOperand, GetOpCodeWithSize(), LHS, RHS);
            else
                throw new Exception(String.Format("Failed to convert assignment expression to string. The operand count {0} is not valid.", op));

            return result.ToLower();
        }
    }
}
