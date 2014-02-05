using System.Collections.Generic;

namespace Zbu.Blocks
{
    class BlockTemp : StructureTemp
    {
        public string Key { get; set; }
        public string Index { get; set; }

        public bool Locked { get; set; }
        public bool Killed { get; set; }

        public string BlockJson { get; set; }

        public readonly List<BlockDataWithLevel> BlockDatas = new List<BlockDataWithLevel>();

        // before? after? replaces? kills?
    }
}