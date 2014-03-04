using System.Collections.Generic;
using Umbraco.Core.Models;

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
        public int Index { get; set; }

        public IDictionary<string, object> Data { get; set; }
        public IPublishedContent Fragment { get; set; }

        // we're going to add to it
        public readonly List<WithLevel<BlockDataValue>> BlockDataValues = new List<WithLevel<BlockDataValue>>();
    }
}