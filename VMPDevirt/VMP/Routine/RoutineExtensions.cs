using Rivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMPDevirt.VMP.ILExpr;

namespace VMPDevirt.VMP.Routine
{
    public static class RoutineExtensions
    {
        public static IEnumerable<ILExpression> GetExpressions(this ILRoutine routine)
        {
            return routine.GetBlocks().SelectMany(x => x.Expressions);
        }

        public static IEnumerable<ILBlock> GetBlocks(this ILRoutine routine)
        {
            return routine.Nodes.Select(x => x.GetBlock());
        }

        public static ILBlock GetBlock(this Node node)
        {
            return (ILBlock)node.UserData.Values.Single();
        }

    }
}
