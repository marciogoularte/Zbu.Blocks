using System.Collections.Generic;

namespace Zbu.Blocks
{
    /// <summary>
    /// A temporary structure used while computing the rendering structure.
    /// </summary>
    class TempStructure
    {
        public string Source { get; set; }

        // we're going to set it at once
        public IEnumerable<TempBlock> Blocks { get; set; }
    }
}