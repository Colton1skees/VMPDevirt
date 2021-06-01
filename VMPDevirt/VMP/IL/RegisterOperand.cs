using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP.IL
{
    public class RegisterOperand : ILOperand
    {
        public override OperandType Type { get; } = OperandType.Register;

        public Register Reg { get; set; }

        public RegisterOperand(Register _reg)
        {
            Reg = _reg;
        }

        public static implicit operator RegisterOperand(Register _reg)
        {
            return new RegisterOperand(_reg);
        }

        public override string ToString()
        {
            return Reg.ToString();
        }
    }
}
