using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMPDevirt.VMP.ILExpr.Operands;

namespace VMPDevirt.VMP.ILExpr
{
    public enum ExprOpCode
    {
        // VM Specific:
        VMENTER,
        VMEXIT,
        READMEM,
        WRITEMEM,
        PUSHVSP,
        SETVSP,

        // x86 temporary helpers:
        PUSHFLAGS,
        COMPUTEFLAGS,

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
        NAND,
        ROL,
        ROR,
    }

    public enum ExprType
    {
        Assignment,
        Stack,
        Special,
    }

    public abstract class ILExpression
    {
        public abstract ExprType Type { get; }

        public ExprOpCode OpCode { get; set; }

        public ExprOperand LHS { get; set; }

        public ExprOperand RHS { get; set; }

        public ulong Address { get; set; }

        public int OpCount => GetOperandCount();

        public int Size => GetSize();

        public IReadOnlyList<ExprOperand> Operands => GetOperands();

        public virtual int GetSize()
        {
            return Operands.Select(x => x.Size).Distinct().Single();
        }

        public int GetOperandCount()
        {
            return GetOperands().Count;
        }

        public IReadOnlyList<ExprOperand> GetOperands()
        {
            List<ExprOperand> operands = new List<ExprOperand>();
            if (HasLHS())
                operands.Add(LHS);
            if (HasRHS())
                operands.Add(RHS);
            return operands.AsReadOnly();
        }

        public bool HasLHS()
        {
            return LHS != null;
        }

        public bool HasRHS()
        {
            return RHS != null;
        }

        public string GetOpCodeWithSize()
        {
            return this.OpCode.ToString() + ":" + this.GetSize();
        }

        public override string ToString()
        {
            var op = OpCount;
            string result = null;
            if (op == 0)
                result = String.Format("{0}", OpCode);
            else if (op == 1)
                result = String.Format("{0} {1}", GetOpCodeWithSize(), LHS);
            else if (op == 2)
                result = String.Format("{0} {1}, {2}", GetOpCodeWithSize(), RHS);
            else
                throw new Exception("Invalid operand count.");

            return result.ToLower(); ;
        }
    }
}
