using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP
{
    public class VMPState
    {
        /// <summary>
        /// The register containing the virtual stack pointer.
        /// </summary>
        public Register VSP { get; }

        /// <summary>
        /// The register containing the bytecode pointer, aka VIP.
        /// </summary>
        public Register VIP { get; }

        /// <summary>
        /// The register containing the virtual context pointer.
        /// </summary>
        public Register VCP { get; }

        /// <summary>
        /// The register containing the virtual rolling key.
        /// </summary>
        public Register VRK { get; set; }

        /// <summary>
        /// The register containing the virtual computation register(usually RAX).
        /// </summary>
        public Register ComputationReg { get; set; }

        public VMPState(Register _regVirtualStack, Register _regVirtualBytecodePointer, Register _regVirtualContext, Register _regVirtualRollingKey, Register _regVirtualComputationRegister)
        {
            VSP = _regVirtualStack;
            VIP = _regVirtualBytecodePointer;
            VCP = _regVirtualContext;
            VRK = _regVirtualRollingKey;
            ComputationReg = _regVirtualComputationRegister;
        }
    }
}
