using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP.IL
{
    public enum ILOpcode
    {
        // VM Specific:
        VMENTER,
        VMEXIT,
        READMEM,
        WRITEMEM,
        VCPOP,
        VCPUSH,
        
        // Stack:
        PUSH,
        POP,

        // Arithmetic:
        ADD,
        SUB,
        MUL,
        DIV,

        // Logical:
        AND,
        OR,
        XOR,
        ROL,
        ROR,
    }

    public class ILInstruction
    {

        public ILOpcode OpCode { get; set; }

        public ILOperand LHS { get; set; }

        public ILOperand RHS { get; set; }

        public ILInstruction(ILOpcode _opCode, ILOperand _lhs, ILOperand _rhs)
        {
            OpCode = _opCode;
            LHS = _lhs;
            RHS = _rhs;
        }

        public bool HasLHS()
        {
            return LHS != null;
        }

        public bool HasRHS()
        {
            return RHS != null;
        }

        public void SetLHS(ulong value)
        {
            LHS = new ImmediateOperand(value);
        }

        public void SetLHS(long value)
        {
            LHS = new ImmediateOperand(value);
        }

        public void SetLHS(Register register)
        {
            LHS = new RegisterOperand(register);
        }

        public void SetRHS(ulong value)
        {
            LHS = new ImmediateOperand(value);
        }

        public void SetRHS(long value)
        {
            LHS = new ImmediateOperand(value);
        }

        public void SetRHS(Register register)
        {
            RHS = new RegisterOperand(register);
        }

    }
}
