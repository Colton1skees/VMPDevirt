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
        /// <summary>
        /// Signed multiplication
        /// </summary>
        IMUL,
        /// <summary>
        /// Unsigned multiplication
        /// </summary>
        UMUL,
        DIV,

        // Logical:
        TRUNC,
        AND,
        OR,
        XOR,
        NAND,
        ROL,
        ROR,
        SHR,
        SHL,

        // MISC:
        MOV,
        COPY
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

        private readonly int internalID;

        private static int globalIDCount;

        private readonly object globalIDLock = new object();

        public int OpCount => GetOperandCount();

        public int Size => GetSize();

        public IReadOnlyList<ExprOperand> Operands => GetOperands();

        protected ILExpression(ExprOpCode _opCode, ExprOperand _lhs = null, ExprOperand _rhs = null)
        {
            OpCode = _opCode;
            LHS = _lhs;
            RHS = _rhs;

            lock(globalIDLock)
            {
                globalIDCount++;
                internalID = globalIDCount;
            }
        }

        public int GetOperandCount()
        {
            return GetOperands().Count;
        }

        public virtual int GetSize()
        {
            return Operands.Select(x => x.Size).Distinct().Single();
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

        public AssignmentExpression Assignment => (AssignmentExpression)this;

        public StackExpression Stack => (StackExpression)this;

        public bool HasLHS()
        {
            return LHS != null;
        }

        public bool HasRHS()
        {
            return RHS != null;
        }

        public bool IsAssignmentExpression()
        {
            return this.Type == ExprType.Assignment;
        }

        public bool IsStackExpression()
        {
            return this.Type == ExprType.Stack;
        }

        public string GetOpCodeWithSize()
        {
            //return this.OpCode.ToString() + ":" + this.GetSize();
            return String.Format("{0} i{1}*", this.OpCode, this.GetSize());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(ILExpression) && !obj.GetType().IsSubclassOf(typeof(ILExpression)))
                return false;

            ILExpression expr = (ILExpression)obj;
            if (expr.internalID != this.internalID)
                return false;

            return true;
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
