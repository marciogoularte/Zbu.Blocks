using System.Collections.Generic;
using System.Linq;

namespace Zbu.Blocks
{
    /// <summary>
    /// Represents a block to be rendered.
    /// </summary>
    public class RenderingBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingBlock"/> class with a source, a key,
        /// a collection of blocks and a content fragment.
        /// </summary>
        /// <param name="source">The block source.</param>
        /// <param name="key">The block key.</param>
        /// <param name="blocks">The block inner blocks.</param>
        /// <param name="fragment">The block content fragment.</param>
        public RenderingBlock(string source, string key, IEnumerable<RenderingBlock> blocks, PublishedContentFragment fragment)
        {
            Source = source;
            Key = key;
            Blocks = blocks.ToArray();
            Fragment = fragment;
        }

        /// <summary>
        /// Gets or sets the source of the block.
        /// </summary>
        /// <remarks>The source determines the view that should be used to render the block.</remarks>
        public string Source { get; private set; }

        /// <summary>
        /// Gets or sets the key of the block.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets the inner blocks collection of the block.
        /// </summary>
        public RenderingBlock[] Blocks { get; private set; }

        /// <summary>
        /// Gets or sets the block content fragment.
        /// </summary>
        public PublishedContentFragment Fragment { get; private set; }
    }
}