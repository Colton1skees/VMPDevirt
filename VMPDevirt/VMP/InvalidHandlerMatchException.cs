using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMPDevirt.VMP
{
    public class InvalidHandlerMatchException : Exception
    {
        public InvalidHandlerMatchException()
        {

        }

        public InvalidHandlerMatchException(string text) : base(text)
        {

        }
    }
}
