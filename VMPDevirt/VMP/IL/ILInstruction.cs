using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP.IL
{
    public enum ILOpcode
    {
        // Special:
        VMEnter,
        VMExit,
        ReadMem,
        WriteMem,
        
        // Stack:
        Push,
        Pop,

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
        private ILOperand lhs;

        private ILOpcode rhs;

        public ILOpcode OpCode { get; set; }

        public ILOperand LHS { get; set; } = new NilOperand();

        public ILOperand RHS { get; set; } = new NilOperand();

        public bool HasLHS()
        {
            return LHS != null;
        }

        public bool HasRHS()
        {
            return RHS != null;
        }

        public ILInstruction()
        {

        }

    }
}
