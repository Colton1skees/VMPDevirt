using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMPDevirt.VMP.ILExpr;
using VMPDevirt.VMP.ILExpr.Operands;
using VMPDevirt.VMP.Routine;

namespace VMPDevirt.Optimization.Passes
{
    public class TemporaryDataFlow
    {
        public AssignmentExpression Assignment { get; set; }

        public List<ILExpression> Readers { get; set; } = new List<ILExpression>();
    }

    /// <summary>
    /// Attempts to perform copy propagation on all temporaries
    /// </summary>
    public class PassCopyPropagation
    {
        private Dictionary<TemporaryOperand, TemporaryDataFlow> dataFlow;

        private ILBlock block;

        public void Execute(ILBlock block)
        {
            this.block = block;
            dataFlow = new Dictionary<TemporaryOperand, TemporaryDataFlow>();

            var operandTemporaries = block.Expressions.SelectMany(x => x.Operands).Where(x => x.IsTemporary()).Cast<TemporaryOperand>().ToList();
            var alternativeTemporaries = block.Expressions.Where(x => x.IsAssignmentExpression() && x.Assignment.DestinationOperand.IsTemporary()).Select(x => x.Assignment.DestinationOperand.Temporary);

            operandTemporaries.AddRange(alternativeTemporaries);
            operandTemporaries = operandTemporaries.Distinct().ToList();
            foreach(var temporary in operandTemporaries)
            {
                dataFlow[temporary] = new TemporaryDataFlow();
            }

            BuildDataFlowGraph();
            PropagateCopies();
        }   

        private void BuildDataFlowGraph()
        {
            foreach(var expr in block.Expressions)
            {
                TrackAssignment(expr);
                TrackReads(expr);
            }
        }

        /// <summary>
        /// Attempts to track all write data about the provided expression
        /// </summary>
        /// <param name="expr"></param>
        private void TrackAssignment(ILExpression expr)
        {
            if(expr.IsAssignmentExpression() && expr.Assignment.DestinationOperand.IsTemporary())
            {
                var assignment = expr.Assignment;
                var temp = assignment.DestinationOperand.Temporary;
                dataFlow[temp].Assignment = expr.Assignment;
            }
        }

        /// <summary>
        /// Attempts to track all read data about the provided expression
        /// </summary>
        /// <param name="expr"></param>
        private void TrackReads(ILExpression expr)
        {
            if(expr.OpCode == ExprOpCode.POP)
            {
                return;
            }

            var temporaryOperands = expr.Operands.Where(x => x.IsTemporary()).Cast<TemporaryOperand>();
            foreach(var temp in temporaryOperands)
            {
                dataFlow[temp].Readers.Add(expr);
            }
        }

        private void PropagateCopies()
        {
            foreach(var pair in dataFlow)
            {
                var temporary = pair.Key;
                var flow = pair.Value;

                // if temp = copy item then:
                //     all expressions which use this temporary get replaced with the original value
                if(flow.Assignment != null && flow.Assignment.OpCode == ExprOpCode.COPY)
                {
                    var originalOperand = flow.Assignment.LHS;
                    foreach(var reader in flow.Readers)
                    {
                        if (reader.LHS == temporary)
                            reader.LHS = originalOperand;

                        if (reader.RHS == temporary)
                            reader.RHS = originalOperand;
                    }

                    block.Expressions.Remove(flow.Assignment);
                }
            }
        }
    }
}
