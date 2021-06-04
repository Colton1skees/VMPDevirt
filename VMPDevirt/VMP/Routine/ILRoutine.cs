using Rivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMPDevirt.VMP.ILExpr;
using VMPDevirt.VMP.ILExpr.Operands;

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
            // Create and initialize the basic block
            ILBlock block = new ILBlock();
            block.Address = address;

            // Create a node, assign the basic block to it, and add it to the graph
            Node node = new Node("0x" + address.ToString("X"));
            node.UserData.Add(address, block);
            Nodes.Add(node);

            // Update block and return it
            block.Node = node;
            block.ParentRoutine = this;
            return block;
        }

        public TemporaryOperand AllocateTemporary(int size)
        {
            int temporaryIndex = 0;
            var temporaries = this.GetBlocks().SelectMany(x => x.Expressions).SelectMany(x => x.Operands).Where(x => x.Type == ExprOperandType.Temporary);
            if(temporaries.Any())
            {
                temporaryIndex = temporaries.Select(x => x.Temporary.ID).Max();
            }

            return new TemporaryOperand(temporaryIndex, size);
        }
    }
}
