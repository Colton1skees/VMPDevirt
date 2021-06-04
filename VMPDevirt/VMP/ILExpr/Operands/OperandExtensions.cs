using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public static class OperandExtensions
    {
        public static int GetSizeInBits(this VirtualRegister vreg)
        {
            switch(vreg)
            {
                case VirtualRegister.RFLAGS:
                    return 64;
                case VirtualRegister.VSP:
                    return 64;
                default:
                    throw new Exception(String.Format("Failed to find size for virtual register: {0}"));
            }
        }
    }
}
