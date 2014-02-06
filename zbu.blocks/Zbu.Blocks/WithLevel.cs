using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbu.Blocks
{
    // temp internal class to attach a level to an immutable block or structure data value.

    class WithLevel<T>
    {
        public WithLevel(T item, int level)
        {
            Item = item;
            Level = level;
        }

        public T Item { get; private set; }
        public int Level { get; private set; }

    }
}
