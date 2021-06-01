using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP.IL
{
    public enum OperandType
    {
        Nil,
        Immediate,
        Register,
    }

    public interface IOperand
    {
        public OperandType Type { get; }
    }
}
