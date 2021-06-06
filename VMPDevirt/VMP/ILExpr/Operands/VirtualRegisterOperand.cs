using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class VirtualRegisterOperand : ExprOperand
    {
        public string Name { get; }
        public int Size { get; }

        public VirtualRegisterOperand(string _name, int _size)
        {
            Name = _name.ToLower();
            Size = _size;
        }

        public override ExprOperandType Type => ExprOperandType.VirtualRegisterOperand;

        public override int GetSize()
        {
            return Size;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(VirtualRegisterOperand))
                return false;

            var vReg = (VirtualRegisterOperand)obj;
            if (vReg.Name != this.Name)
                return false;

            return true;
        }

        public override string ToString()
        {
            return Name.ToLower();
        }
    }
}
