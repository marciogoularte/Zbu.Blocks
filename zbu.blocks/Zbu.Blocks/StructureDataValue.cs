using System.Collections.Generic;
using System.ComponentModel;

namespace Zbu.Blocks
{
    /// <summary>
    /// Represents the JSON structure data value.
    /// </summary>
    /// <remarks>The Umbraco DataType returns an enumeration of <c>StructureData</c> instances.</remarks>
    public class StructureDataValue
    {
        private readonly static BlockDataValue[] NoBlocks = {};
        public static readonly string[] NoContexts = {};
        public static readonly string[] NoContentTypes = { };

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureDataValue"/> class.
        /// </summary>
        public StructureDataValue()
        {
            // initialize with default values
            Description = string.Empty;
            Name = string.Empty;
            Source = string.Empty;
            IsReset = false;
            MinLevel = 0;
            MaxLevel = int.MaxValue;
            Blocks = NoBlocks;
            Data = null;
            Contexts = NoContexts;
            ContentTypes = NoContentTypes;
            ContentTypesNegate = false;
        }

        /// <summary>
        /// Gets or sets the description of the structure.
        /// </summary>
        /// <remarks>Description is free text.</remarks>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name of the structure.
        /// </summary>
        public string Name { get { return _name; } set { _name = (value ?? string.Empty).ToLowerInvariant(); } }
        private string _name;

        /// <summary>
        /// Gets or sets the source of the structure.
        /// </summary>
        public string Source { get { return _source; } set { _source = (value ?? string.Empty).ToLowerInvariant(); } }
        private string _source;

        /// <summary>
        /// Gets or sets a value indicating whether the structure resets the chain,
        /// ie whether structures above the current structure should be ignored.
        /// </summary>
        public bool IsReset { get; set; }

        /// <summary>
        /// Gets or sets the minimum level at which the structure applies.
        /// </summary>
        public int MinLevel { get; set; }

        /// <summary>
        /// Gets or sets the maximum level at which the structure applies.
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Gets or sets the structure data.
        /// </summary>
        public IDictionary<string, object> Data { get; set; }

        /// <summary>
        /// Gets or sets the context keys for which the structure applies.
        /// </summary>
        public string[] Contexts { get; set; }

        /// <summary>
        /// Gets or sets the content types for which the structure applies.
        /// </summary>
        public string[] ContentTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to negate the content types filter.
        /// </summary>
        public bool ContentTypesNegate { get; set; }

        /// <summary>
        /// Gets or sets the blocks collection of the structure.
        /// </summary>
        public BlockDataValue[] Blocks { get; set; }

        // ensures that blocks fragments have been initialized
        // is invoked by the converter after the whole structure property has been parsed
        public void EnsureFragments(bool preview)
        {
            foreach (var block in Blocks)
                block.EnsureFragments(preview);
        }
    }
}