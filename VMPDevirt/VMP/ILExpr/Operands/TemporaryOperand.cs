using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class TemporaryOperand : ExprOperand
    {
        public override ExprOperandType Type => ExprOperandType.Temporary;

        public int ID { get; }

        private int size;

        public TemporaryOperand(int _id, int _size)
        {
            ID = _id;
            size = _size;
        }

        public override int GetSize()
        {
            return size;
        }

        public override string ToString()
        {
            return "%t" + ID.ToString();
        }
    }
}
