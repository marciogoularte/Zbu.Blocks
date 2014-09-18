using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbu.Blocks.Mvc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class BlockControllerAttribute : Attribute
    {
        public string BlockSource { get; private set; }

        public BlockControllerAttribute(string blockSource)
        {
            BlockSource = blockSource;
        }
    }
}
