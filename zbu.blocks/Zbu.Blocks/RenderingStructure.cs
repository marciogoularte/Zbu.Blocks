using System.Collections.Generic;
using System.Linq;

namespace Zbu.Blocks
{
    /// <summary>
    /// Represents a structure to be rendered.
    /// </summary>
    public class RenderingStructure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingStructure"/> class with a source and a collection of blocks.
        /// </summary>
        /// <param name="source">The structure source.</param>
        /// <param name="blocks">The structure blocks.</param>
        public RenderingStructure(string source, IEnumerable<RenderingBlock> blocks)
        {
            Source = source;
            Blocks = blocks.ToArray();
        }

        /// <summary>
        /// Gets or sets the source of the structure.
        /// </summary>
        /// <remarks>The source determines the view that should be used to render the structure.</remarks>
        public string Source { get; private set; }

        /// <summary>
        /// Gets or sets the blocks collection of the structure.
        /// </summary>
        public RenderingBlock[] Blocks { get; private set; }
    }
}