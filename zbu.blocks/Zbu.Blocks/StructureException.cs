using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbu.Blocks
{
    public class StructureException : Exception
    {
        public StructureException(string message)
            : base(message)
        {}

        public StructureException(string message, params object[] args)
            : base(string.Format(message, args))
        {}
    }
}
