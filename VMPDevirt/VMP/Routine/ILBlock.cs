using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMPDevirt.VMP.ILExpr;

namespace VMPDevirt.VMP.Routine
{
    public class ILBlock
    {
        private List<ILExpression> expressions = new List<ILExpression>();

        /// <summary>
        /// Gets or sets the virtual address of this basic block.
        /// </summary>
        public ulong Address { get; set; }

        /// <summary>
        /// Gets or sets the list of expression inside this basic block.
        /// </summary>
        public List<ILExpression> Expressions { get; set; }

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
            expressions.RemoveAt(expressions.Count - 1);
        }
    }
}
