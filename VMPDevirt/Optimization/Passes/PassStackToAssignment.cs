using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMPDevirt.VMP.ILExpr;

namespace VMPDevirt.Optimization.Passes
{

    public class OptimizedPushPopSequence
    {
        public ILExpression PushExpr { get; set; }

        public ILExpression PopExpr { get; set; }

        /// <summary>
        /// Represents the optimized assignment statement which the push/pop sequence was transformed into.
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

        public void Execute(List<ILExpression> expressions)
        {

            pushExpressions = new Stack<ILExpression>();
            foreach (var expr in expressions)
            {
                if (expr.OpCode == ExprOpCode.PUSH)
                    TrackPush(expr);

                else if (expr.OpCode == ExprOpCode.POP)
                    TrackPush(expr);
            }
        }

        private void TrackPush(ILExpression pushExpr)
        {
            pushExpressions.Push(pushExpr);
        }

        private void TrackPop(ILExpression popExpr)
        {
            if (!pushExpressions.Any())
            {
                OptimizationLogger.LogWeirdBehavior("Encountered POP with unknown source. This most likely means that the current block accesses an item from outside of the block");
            }

            else
            {

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ILExpression ConvertPushPopSequenceToTransfer(ILExpression pushExpr, ILExpression popExpr)
        {
            return null;
        }
    }
}
