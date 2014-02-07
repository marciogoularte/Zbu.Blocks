using System.Collections.Generic;

namespace Zbu.Blocks
{
    /// <summary>
    /// A temporary block used while computing the rendering structure.
    /// </summary>
    class TempBlock : TempStructure
    {
        public string Name { get; set; }

        public bool Locked { get; set; }
        public bool Killed { get; set; }

        public string DataJson { get; set; }
        public string FragmentJson { get; set; }

        // we're going to add to it
        public readonly List<WithLevel<BlockDataValue>> BlockDataValues = new List<WithLevel<BlockDataValue>>();
    }
}