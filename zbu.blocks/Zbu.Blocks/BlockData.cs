using Newtonsoft.Json;

namespace Zbu.Blocks
{
    /// <summary>
    /// Represent the JSON block data.
    /// </summary>
    public class BlockData
    {
        private readonly static BlockData[] NoBlocks = { };
        private string _source;
        private string _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockData"/> class.
        /// </summary>
        public BlockData()
        {
            // initialize with default values
            _source = "default";
            _key = string.Empty;
            SourceKey = "default";
            Name = string.Empty;
            IsUnique = false;
            IsKill = false;
            IsReset = false;
            MinLevel = 0;
            MaxLevel = int.MaxValue;
            Blocks = NoBlocks;
            FragmentJson = string.Empty;
        }

        private void InitSourceKey()
        {
            SourceKey = _source + (string.IsNullOrWhiteSpace(_key) ? string.Empty : ("+" + _key));
        }

        /// <summary>
        /// Gets or sets the source of the block.
        /// </summary>
        public string Source { get { return _source; } set { _source = value; InitSourceKey(); } }

        /// <summary>
        /// Gets or sets the unique key of the block.
        /// </summary>
        public string Key { get { return _key; } set { _key = value; InitSourceKey(); } }

        /// <summary>
        /// Gets or sets the source-key of the block.
        /// </summary>
        /// <remarks>The source-key is a combination of the source and the key.</remarks>
        [JsonIgnore]
        public string SourceKey { get; private set; }

        /// <summary>
        /// Gets or sets the friendly name of the block.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the block is unique.
        /// </summary>
        /// <remarks>Only the first, top-most, instance of a block in a structure
        /// can be marked as unique. Blocks belonging to non-unique blocks cannot be
        /// unique.</remarks>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the block kills an existing block.
        /// </summary>
        public bool IsKill { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the block resets the chain, 
        /// ie whether above the current block should be ignored.
        /// </summary>
        /// <remarks>Makes sense only for unique blocks.</remarks>
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
        /// Gets or sets the inner blocks collection of the block.
        /// </summary>
        public BlockData[] Blocks { get; set; }

        /// <summary>
        /// Gets or sets the block content fragment JSON.
        /// </summary>
        public string FragmentJson { get; set; }
    }
}