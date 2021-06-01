using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP.IL
{
    public class NilOperand : ILOperand
    {
        public override OperandType Type { get; } = OperandType.Nil;

        public override string ToString()
        {
            return "NIL";
        }
    }
}
