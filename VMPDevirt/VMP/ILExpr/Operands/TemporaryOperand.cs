﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VMPDevirt.VMP.ILExpr.Operands
{
    public class TemporaryOperand : ExprOperand
    {
        public override ExprOperandType Type => ExprOperandType.Temporary;

        public int ID { get; }

        private int size;

        public TemporaryOperand(int _id, int _size)
        {
            ID = _id;
            size = _size;
        }

        public override int GetSize()
        {
            return size;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(TemporaryOperand))
                return false;

            TemporaryOperand expr = (TemporaryOperand)obj;
            if (expr.ID != this.ID)
                return false;

            return true;
        }

        public override string ToString()
        {
            return "%t" + ID.ToString();
        }
    }
}
