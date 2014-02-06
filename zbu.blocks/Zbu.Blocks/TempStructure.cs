namespace Zbu.Blocks
{
    /// <summary>
    /// A temporary structure used while computing the rendering structure.
    /// </summary>
    class TempStructure
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public TempBlock[] Blocks { get; set; }
    }
}