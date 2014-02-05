namespace Zbu.Blocks
{
    class StructureDataWithLevel
    {
        public StructureDataWithLevel(StructureData structureData, int level)
        {
            StructureData = structureData;
            Level = level;
        }

        public StructureData StructureData { get; private set; }
        public int Level { get; private set; }
    }
}