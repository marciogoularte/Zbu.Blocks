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
    public class RenderingBlockCollection : IEnumerable<RenderingBlock>
    {
        private readonly IDictionary<string, RenderingBlock> _blocks
            = new Dictionary<string, RenderingBlock>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingBlockCollection"/> class with blocks.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        public RenderingBlockCollection(IEnumerable<RenderingBlock> blocks)
        {
            // not every block has a name
            // for those that don't have a name, we need to create one
            // which hopefully will not collide with actual block names

            var i = 0;
            foreach (var block in blocks)
            {
                var key = string.IsNullOrWhiteSpace(block.Name)
                    ? "___block." + (i++) + "___"
                    : block.Name;
                _blocks[key] = block;
            }
        }

        /// <summary>
        /// Gets a rendering block by its name.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        /// <returns>The corresponding block, if any, else <c>null</c>.</returns>
        /// <remarks>The name is case-insensitive.</remarks>
        public RenderingBlock this[string name]
        {
            get
            {
                RenderingBlock block;
                return _blocks.TryGetValue(name, out block) ? block : null;
            }
        }

        public RenderingBlock this[int index]
        {
            get { return _blocks.ElementAtOrDefault(index).Value; }
        }

        /// <summary>
        /// Gets the number of blocks contained in the collection.
        /// </summary>
        public int Count
        {
            get { return _blocks.Count; }
        }

        public IEnumerator<RenderingBlock> GetEnumerator()
        {
            return _blocks.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _blocks.Values.GetEnumerator();
        }
    }
}
