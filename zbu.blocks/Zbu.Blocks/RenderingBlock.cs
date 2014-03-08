using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Zbu.Blocks
{
    /// <summary>
    /// Represents a block to be rendered.
    /// </summary>
    /// <remarks>Name makes a block named ie unique, Type initialized all properties from a predefined
    /// type, Source overrides name if needed. Such as when using the same source for two types of blocks
    /// which would be differenciated by their name.</remarks>
    public class RenderingBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingBlock"/> class with a name, a source,
        /// a collection of blocks, and data and fragment json.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        /// <param name="source">The block source.</param>
        /// <param name="blocks">The block inner blocks.</param>
        /// <param name="data">The block data dictionary (using case-insensitive keys).</param>
        /// <param name="fragment">The block content fragment.</param>
        /// <remarks>The block data can be null.</remarks>
        public RenderingBlock(string name, string source, IEnumerable<RenderingBlock> blocks, 
            IDictionary<string, object> data, IPublishedContent fragment)
        {
            Name = name;
            Source = source;
            Blocks = new RenderingBlockCollection(blocks);
            Data = data;
            Fragment = fragment;
        }

        /// <summary>
        /// Gets or sets the name of the block.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the source of the block.
        /// </summary>
        /// <remarks>The source determines the view that should be used to render the block.</remarks>
        public string Source { get; private set; }

        /// <summary>
        /// Gets or sets the inner blocks collection of the block.
        /// </summary>
        public RenderingBlockCollection Blocks { get; private set; }

        /// <summary>
        /// Gets or sets the block data dictionary.
        /// </summary>
        /// <remarks>The dictionary uses case-insensitive keys.</remarks>
        public IDictionary<string, object> Data{ get; private set; }

        /// <summary>
        /// Gets or sets the block content fragment.
        /// </summary>
        public IPublishedContent Fragment { get; private set; }

        public T GetData<T>(string key)
        {
            return GetData(key, default(T));
        }

        public T GetData<T>(string key, T defaultValue)
        {
            if (Data == null) return defaultValue;
            object o;
            if (!Data.TryGetValue(key, out o)) return defaultValue;
            return o is T ? (T) o : defaultValue;
        }
    }
}