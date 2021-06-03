using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.IL
{
    public class TemporaryOperand : ILOperand
    {
        public override OperandType Type { get; } = OperandType.Temporary;

        public int ID { get; }

        public TemporaryOperand(int _id)
        {
            ID = _id;
        }

        public override string ToString()
        {
            return "%t" + ID.ToString();
        }
    }
}
