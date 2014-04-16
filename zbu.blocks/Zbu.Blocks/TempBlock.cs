using System;
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

        public bool Killed { get; set; }
        public int Index { get; set; }

        // data dictionary uses case-insensitive keys
        public IDictionary<string, object> Data { get; private set; }
        public IPublishedContent Fragment { get; set; }
        public CacheProfile Cache { get; set; }

        public void MergeData(IEnumerable<KeyValuePair<string, object>> data)
        {
            if (data == null)
                return;
            if (Data == null)
                Data = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var kvp in data)
                Data[kvp.Key] = kvp.Value;
        }

        // we're going to add to it
        public readonly List<WithLevel<BlockDataValue>> BlockDataValues = new List<WithLevel<BlockDataValue>>();
    }
}