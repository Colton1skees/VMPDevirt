using Rivers;
using System;
using System.Collections.Generic;
using System.Text;
using VMPDevirt.VMP.ILExpr;

namespace VMPDevirt.VMP.Routine
{
    /// <summary>
    /// Represents the control flow graph of a routine/function, with some additional context.
    /// </summary>
    public class ILRoutine : Graph
    {
        public ulong Address { get; set; }

        public ILRoutine(ulong _address)
        {
            Address = _address;
        }

        /// <summary>
        /// Appends a basic block with the provided expressions to the current routine.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public ILBlock AllocateBlock(ulong address, List<ILExpression> expressions)
        {
            var block = AllocateEmptyBlock(address);
            block.Expressions = expressions;
            return block;
        }

        /// <summary>
        /// Appends a new and empty basic block to the current routine.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public ILBlock AllocateEmptyBlock(ulong address)
        {
            // Create the basic block
            ILBlock block = new ILBlock();
            block.Address = address;

            // Create a node, assign the basic block to it, and add it to the graph
            Node node = new Node("0x" + address.ToString("X"));
            node.UserData.Add(address, block);
            Nodes.Add(node);
            return block;
        }
    }
}
