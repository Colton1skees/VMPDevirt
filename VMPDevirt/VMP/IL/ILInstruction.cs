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

        // x86 temporary helpers:
        PUSHFLAGS,
        COMPUTEFLAGS,

        /// <summary>
        /// Pops a value from the virtual stack into the virtual context.
        /// </summary>
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

        public IReadOnlyList<ILOperand> Operands 
        { 
            get
            {
                List<ILOperand> operands = new List<ILOperand>();
                if (HasLHS())
                    operands.Add(LHS);
                if (HasRHS())
                    operands.Add(RHS);

                return operands.AsReadOnly();
            }
        }

        public int OpCount 
        { 
            get
            {
                return Operands.Count;
            }
        }

        public ILInstruction(ILOpcode _opCode, ILOperand _lhs = null, ILOperand _rhs = null)
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

        public override string ToString()
        {
            string outputString;

            if (OpCount == 0)
                outputString = OpCode.ToString();
            else if (OpCount == 1)
                outputString = String.Format("{0} {1}", OpCode, LHS);
            else if (OpCount == 2)
                outputString = String.Format("{0} {1}, {2}", OpCode, LHS, RHS);
            else
                throw new Exception();

            return outputString.ToLower();
        }

    }
}
