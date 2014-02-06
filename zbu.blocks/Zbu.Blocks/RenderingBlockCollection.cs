using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbu.Blocks
{
    /// <summary>
    /// Represents a collection of <see cref="RenderingBlock"/>.
    /// </summary>
    /// <remarks>A collection of rendering blocks is a list, plus an additional
    /// indexer by name to retrieve named blocks.</remarks>
    public class RenderingBlockCollection : List<RenderingBlock>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingBlockCollection"/> class with blocks.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        public RenderingBlockCollection(IEnumerable<RenderingBlock> blocks)
            : base(blocks)
        { }

        /// <summary>
        /// Gets a rendering block by its name.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        /// <returns>The corresponding block, if any, else <c>null</c>.</returns>
        public RenderingBlock this[string name]
        {
            get
            {
                name = name.ToLowerInvariant();
                return this.FirstOrDefault(x => x.Name == name);
            }
        }
    }
}
