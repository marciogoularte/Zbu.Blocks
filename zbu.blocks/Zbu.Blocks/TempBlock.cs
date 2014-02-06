using System.Collections.Generic;

namespace Zbu.Blocks
{
    /// <summary>
    /// A temporary block used while computing the rendering structure.
    /// </summary>
    class TempBlock : TempStructure
    {
        public bool Locked { get; set; }
        public bool Killed { get; set; }

        public string DataJson { get; set; }
        public string FragmentJson { get; set; }

        public readonly List<WithLevel<BlockDataValue>> BlockDatas = new List<WithLevel<BlockDataValue>>();
    }
}