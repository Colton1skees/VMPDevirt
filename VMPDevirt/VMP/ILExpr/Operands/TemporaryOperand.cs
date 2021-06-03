using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class TemporaryOperand : ExprOperand
    {
        public override ExprOperandType Type => ExprOperandType.Temporary;

        public int ID { get; }

        public TemporaryOperand(int _id)
        {
            ID = _id;
        }

        public override string ToString()
        {
            return "t" + ID.ToString();
        }

        public override int GetSize()
        {
            throw new NotImplementedException();
        }
    }
}
