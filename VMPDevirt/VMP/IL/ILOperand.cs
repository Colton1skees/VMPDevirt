using Iced.Intel;
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

    public abstract class ILOperand
    {
        public abstract OperandType Type { get; }

        public ImmediateOperand Immediate
        {
            get 
            {
                return (ImmediateOperand)this; 
            }
        }

        public RegisterOperand Register
        {
            get
            { 
                return (RegisterOperand)this;
            }
        }

        public bool IsImmediate()
        {
            return Type == OperandType.Immediate;
        }

        public bool IsReg()
        {
            return Type == OperandType.Register;
        }


        public static ILOperand Create(ulong value)
        {
            return new ImmediateOperand(value);
        }

        public static ILOperand Create(long value)
        {
            return new ImmediateOperand(value);
        }

        public static ILOperand Create(Register register)
        {
            return new RegisterOperand(register);
        }

    }
}
