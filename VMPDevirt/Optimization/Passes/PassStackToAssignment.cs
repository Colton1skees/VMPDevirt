using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMPDevirt.VMP.ILExpr;
using VMPDevirt.VMP.Routine;

namespace VMPDevirt.Optimization.Passes
{
    public class OptimizedPushPopSequence
    {
        /// <summary>
        /// Gets or sets the original push expression in the push/pop sequence.
        /// </summary>
        public ILExpression PushExpr { get; set; }

        /// <summary>
        /// Gets or sets the original pop expression in the push/pop sequence.
        /// </summary>
        public ILExpression PopExpr { get; set; }

        /// <summary>
        /// Gets or sets the optimized assignment statement which the push/pop sequence was transformed into.
        /// </summary>
        public ILExpression OptimizedAssignment { get; set; }
    }

    /// <summary>
    /// Provides an optimization pass which attempts to convert push/pop sequences into assignment statements.
    /// 
    /// Input:
    ///     push i64* 0x14000
    ///     pop i64* t0
    ///     
    /// Output:
    ///     t0 = copy 0x14000
    /// </summary>
    public class PassStackToAssignment
    {
        private Stack<ILExpression> pushExpressions;

        private List<OptimizedPushPopSequence> sequences = new List<OptimizedPushPopSequence>();

        private ILBlock block;

        public void Execute(ILBlock block)
        {
            pushExpressions = new Stack<ILExpression>();
            sequences = new List<OptimizedPushPopSequence>();
            this.block = block;

            List<ILExpression> expressionsToRemove = new List<ILExpression>();
            foreach (var expr in block.Expressions)
            {
                if (expr.OpCode == ExprOpCode.PUSH)
                {
                    TrackPush(expr);
                }

                else if (expr.OpCode == ExprOpCode.POP)
                {
                    TrackPop(expr);
                }

                else if (expr.OpCode == ExprOpCode.READMEM)
                {
                    var pushExpr = pushExpressions.Pop();
                    expressionsToRemove.Add(pushExpr);
                    // TODO: Validate if the push expression is actually used....

                }
            }


            foreach (var expr in expressionsToRemove)
            {
                block.RemoveExpression(expr);
            }

            if (pushExpressions.Any())
            {
                throw new Exception(String.Format("Failed to optimize stack assignments. Encountered {0} unhandled pops", pushExpressions.Count));
            }

            UpdateBlockWithOptimizations();

        }

        private void TrackPush(ILExpression pushExpr)
        {
            pushExpressions.Push(pushExpr);
        }

        private void TrackPop(ILExpression popExpr)
        {
            // If we encounter a pop without any prior tracked pushes, then it is a cross-block access (or invalid optimization...).
            if (!pushExpressions.Any())
            {
                OptimizationLogger.LogWeirdBehavior("Encountered POP with unknown source. This most likely means that we encountered a cross-block access (or an invalid optimization..)");
            }

            // If we encouter a push and pop sequence, then we create an optimized assignment sequence and store the input expressions for later removal.
            else
            {
                OptimizedPushPopSequence optimizedSequence = new OptimizedPushPopSequence();
                optimizedSequence.PushExpr = pushExpressions.Pop();
                optimizedSequence.PopExpr = popExpr;
                optimizedSequence.OptimizedAssignment = new AssignmentExpression(ExprOpCode.COPY, popExpr.Op1, optimizedSequence.PushExpr.Op1);
                sequences.Add(optimizedSequence);

                if (optimizedSequence.PushExpr.Size != optimizedSequence.PopExpr.Size)
                    throw new Exception();
            }

        }

        /// <summary>
        /// Converts a push pop sequence into a single assignment
        /// </summary>
        /// <returns></returns>
        public OptimizedPushPopSequence GenerateOptimizedPushPopSequence(ILExpression pushExpr, ILExpression popExpr)
        {
            OptimizedPushPopSequence optimizedSequence = new OptimizedPushPopSequence();
            optimizedSequence.PushExpr = pushExpr;
            optimizedSequence.PopExpr = popExpr;
            optimizedSequence.OptimizedAssignment = new AssignmentExpression(ExprOpCode.COPY, popExpr.Op1, optimizedSequence.PushExpr.Op1);
            return optimizedSequence;
        }

        /// <summary>
        /// Takes the optimized sequences/assignments and updates the basic block to include the changes
        /// </summary>
        public void UpdateBlockWithOptimizations()
        {
            foreach(var sequence in sequences)
            {
                block.RemoveExpression(sequence.PushExpr);
                block.ReplaceExpression(sequence.PopExpr, sequence.OptimizedAssignment);
            }
        }
    }
}
