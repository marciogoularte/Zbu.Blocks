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
        /// <param name="dataJson">The block data json.</param>
        /// <param name="fragmentJson">The block content fragment json.</param>
        public RenderingBlock(string name, string source, IEnumerable<RenderingBlock> blocks, 
            string dataJson, string fragmentJson)
        {
            Name = name;
            Source = source;
            Blocks = new RenderingBlockCollection(blocks);
            Data = dataJson == null ? null : CreateDataFromJson(dataJson);
            Fragment = fragmentJson == null ? null : CreateFragmentFromJson(fragmentJson);
        }

        // fixme - move to ctor
        Dictionary<string, object> CreateDataFromJson(string json)
        {
            // fixme - creating far too many serializers!
            var serializer = new JsonSerializer();
            return serializer.Deserialize<Dictionary<string, object>>(json);
        }

        // fixme
        static IPublishedContent CreateFragmentFromJson(string json)
        {
            return null;
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
        /// Gets or sets the block content fragment.
        /// </summary>
        public IDictionary<string, object> Data{ get; private set; }

        /// <summary>
        /// Gets or sets the block content fragment.
        /// </summary>
        public IPublishedContent Fragment { get; private set; }
    }
}