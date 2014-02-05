namespace Zbu.Blocks
{
    internal class BlockDataWithLevel
    {
        public BlockDataWithLevel(BlockData blockData, int level)
        {
            BlockData = blockData;
            Level = level;
        }

        public BlockData BlockData { get; private set; }
        public int Level { get; private set; }
    }
}