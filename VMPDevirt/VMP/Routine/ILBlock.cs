using Rivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMPDevirt.VMP.ILExpr;
using VMPDevirt.VMP.ILExpr.Operands;

namespace VMPDevirt.VMP.Routine
{
    public class ILBlock
    {
        /// <summary>
        /// Gets or sets the virtual address of this basic block.
        /// </summary>
        public ulong Address { get; set; }

        public Node Node { get; set; }

        public ILRoutine ParentRoutine { get; set; }

        /// <summary>
        /// Gets or sets the list of expression inside this basic block.
        /// </summary>
        public List<ILExpression> Expressions { get; set; } = new List<ILExpression>();

        /// <summary>
        /// Gets the first expression inside this basic block.
        /// </summary>
        public ILExpression EntryExpression => Expressions.First();

        /// <summary>
        /// Gets the last expression inside this basic block.
        /// </summary>
        public ILExpression ExitExpression => Expressions.Last();

        /// <summary>
        /// Sets the last expression of this basic block
        /// </summary>
        /// <param name="expr"></param>
        public void SetExitExpression(ILExpression expr)
        {
            Expressions.RemoveAt(Expressions.Count - 1);
        }

        public TemporaryOperand AllocateTemporary(int size)
        {
            return ParentRoutine.AllocateTemporary(size);
        }

        public void RemoveExpression(ILExpression expr)
        {
            Expressions.Remove(expr);
        }

        public void ReplaceExpression(ILExpression oldExpr, ILExpression newExpr)
        {
            var index = Expressions.IndexOf(oldExpr);
            if(index != -1)
            {
                Expressions[index] = newExpr;
            }

            // throw exceptions if there are any duplicates(this should be removed later, only temporarily validating something...)
            index = Expressions.IndexOf(oldExpr);
            if (index != -1)
                throw new Exception();
        }

    }
}
