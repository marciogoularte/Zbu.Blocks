namespace Zbu.Blocks
{
    /// <summary>
    /// Represents the JSON structure data.
    /// </summary>
    public class StructureData
    {
        private readonly static BlockData[] NoBlocks = {};

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureData"/> class.
        /// </summary>
        public StructureData()
        {
            // initialize with default values
            Source = "default";
            Name = string.Empty;
            IsReset = false;
            MinLevel = 0;
            MaxLevel = int.MaxValue;
            Blocks = NoBlocks;
        }

        /// <summary>
        /// Gets or sets the source of the structure.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the friendly name of the structure.
        /// </summary>
        public string Name { get; set; }

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
        /// Gets or sets the blocks collection of the structure.
        /// </summary>
        public BlockData[] Blocks { get; set; }
    }
}