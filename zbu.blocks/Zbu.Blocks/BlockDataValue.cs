using Newtonsoft.Json;

namespace Zbu.Blocks
{
    /// <summary>
    /// Represent the JSON block data.
    /// </summary>
    public class BlockDataValue
    {
        private readonly static BlockDataValue[] NoBlocks = { };

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDataValue"/> class.
        /// </summary>
        public BlockDataValue()
        {
            // initialize with default values
            Description = string.Empty;
            Name = string.Empty;
            Type = string.Empty;
            Source = string.Empty;
            IsKill = false;
            IsReset = false;
            MinLevel = 0;
            MaxLevel = int.MaxValue;
            Blocks = NoBlocks;
            DataJson = string.Empty;
            FragmentJson = string.Empty;
        }

        /// <summary>
        /// Gets or sets the description of the block.
        /// </summary>
        /// <remarks>Description is free text.</remarks>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name of the block.
        /// </summary>
        /// <remarks>Naming a block makes it unique. Name can contain slashes. Only
        /// top-level blocks, or named blocks, can contain named blocks.</remarks>
        public string Name { get { return _name; } set { _name = (value ?? string.Empty).ToLowerInvariant(); } }
        private string _name;

        /// <summary>
        /// Gets a value indicating whether the block is named.
        /// </summary>
        [JsonIgnore]
        public bool IsNamed
        {
            get { return !string.IsNullOrWhiteSpace(_name); }
        }

        /// <summary>
        /// Gets or sets the type of the block.
        /// </summary>
        /// <remarks>A block with a type will initialize with values from config.</remarks>
        public string Type { get { return _type; } set { _type = (value ?? string.Empty).ToLowerInvariant(); } }
        private string _type;

        /// <summary>
        /// Gets or sets the source of the block.
        /// </summary>
        /// <remarks>The source is the actual view to render. Can contain slashes. By
        /// default it derives from the name if there's a name.</remarks>
        public string Source { get { return _source; } set { _source = (value ?? string.Empty).ToLowerInvariant(); } }
        private string _source;

        /// <summary>
        /// Gets or sets a value indicating whether the block kills an existing block.
        /// </summary>
        public bool IsKill { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the block resets the chain, 
        /// ie whether above the current block should be ignored.
        /// </summary>
        /// <remarks>Makes sense only for unique (named) blocks.</remarks>
        public bool IsReset { get; set; }

        /// <summary>
        /// Gets or sets the minimum level at which the block applies.
        /// </summary>
        public int MinLevel { get; set; }

        /// <summary>
        /// Gets or sets the maximum level at which the block applies.
        /// </summary>
        public int MaxLevel { get; set; }

        // fixme
        // should we handle Index, Before, After to sort blocks?!

        /// <summary>
        /// Gets or sets the block data JSON.
        /// </summary>
        /// <remarks>Deserializes into a dictionary.</remarks>
        public string DataJson { get; set; }

        /// <summary>
        /// Gets or sets the block content fragment JSON.
        /// </summary>
        /// <remarks>Deserializes into content fragment.</remarks>
        public string FragmentJson { get; set; }

        /// <summary>
        /// Gets or sets the inner blocks collection of the block.
        /// </summary>
        public BlockDataValue[] Blocks { get; set; }
    }
}