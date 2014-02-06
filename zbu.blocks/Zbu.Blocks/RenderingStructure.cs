using System;
using System.Collections.Generic;
using System.Linq;

namespace Zbu.Blocks
{
    /// <summary>
    /// Represents a structure to be rendered.
    /// </summary>
    public class RenderingStructure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingStructure"/> class with a source and a collection of blocks.
        /// </summary>
        /// <param name="source">The structure source.</param>
        /// <param name="blocks">The structure blocks.</param>
        public RenderingStructure(string source, IEnumerable<RenderingBlock> blocks)
        {
            Source = source;
            Blocks = new RenderingBlockCollection(blocks);
        }

        /// <summary>
        /// Gets or sets the source of the structure.
        /// </summary>
        /// <remarks>The source determines the view that should be used to render the structure.</remarks>
        public string Source { get; private set; }

        /// <summary>
        /// Gets or sets the blocks collection of the structure.
        /// </summary>
        public RenderingBlockCollection Blocks { get; private set; }

        #region Compute

        public static RenderingStructure Compute(PublishedContent content, Func<PublishedContent, IEnumerable<StructureDataValue>> propertyAccessor)
        {
            // get the structures that apply to the content
            // for each structure, remember its relative level
            //   so that we can use it to filter the blocks
            // sort order is bottom-top
            var structureDataValues = new List<WithLevel<StructureDataValue>>();
            var level = 0;
            while (content != null)
            {
                var contentStructureDataValues = propertyAccessor(content);
                if (contentStructureDataValues == null)
                {
                    // nothing here, move up
                    level += 1;
                    content = content.Parent;
                    continue;
                }

                var l = level;
                foreach (var s in contentStructureDataValues
                    .Where(x => x.MinLevel <= l && x.MaxLevel >= l)
                    .Reverse())
                {
                    structureDataValues.Add(new WithLevel<StructureDataValue>(s, l));
                    if (!s.IsReset) continue;
                    content = null;
                    break;
                }

                level += 1;
                content = content == null ? null : content.Parent;
            }

            // gets the top-level blocks that apply to the content
            // for each block, remember its relative level
            //   so that we can use it to filter the sub-blocks
            // sort order is bottom-top
            var blockDataValues = structureDataValues // bottom-top
                .SelectMany(s => s
                    .Item.Blocks // top-bottom
                        .Where(x => x.MinLevel <= s.Level && x.MaxLevel >= s.Level)
                        .Reverse() // bottom-top
                        .Select(x => new WithLevel<BlockDataValue>(x, s.Level)))
                .ToArray();

            // create the temporary structure
            var tempStructure = new TempStructure
            {
                Source = structureDataValues
                    .Select(x => string.IsNullOrEmpty(x.Item.Source) ? x.Item.Name : x.Item.Source) // pick .Source else fallback to .Name
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) // pick the first that's not empty
                         ?? "default", // if everything fails then fall back to "default"

                Blocks = GetTempFromData(blockDataValues).Reverse().ToArray()
            };

            return GetRenderingFromTemp(tempStructure);
        }

        private static IEnumerable<TempBlock> GetTempFromData(IEnumerable<WithLevel<BlockDataValue>> blockDataValues)
        {
            // the array is bottom-top
            var blockDataValuesArray = blockDataValues.ToArray(); // iterate only once

            // find named blocks
            // because we're iterating bottom-top, this will get the top-most of each name
            var namedBlockDataValues = new Dictionary<string, BlockDataValue>();
            foreach (var b in blockDataValuesArray.Where(x => x.Item.IsNamed).Select(x => x.Item))
                namedBlockDataValues[b.Name] = b;

            // create the temporary named blocks from the top-most data value
            var namedTempBlocks = namedBlockDataValues.Values
                .ToDictionary(
                    blockDataValue => blockDataValue.Name,
                    blockDataValue => new TempBlock
                        {
                            Name = blockDataValue.Name,
                            Source = blockDataValue.Source, 
                            DataJson = blockDataValue.DataJson,
                            FragmentJson = blockDataValue.FragmentJson
                        });

            // build the temporary blocks list
            var tempBlocks = new List<TempBlock>();
            foreach (var block in blockDataValuesArray) // bottom-top
            {
                var blockDataValue = block.Item;
                var blockLevel = block.Level;

                // check if it matches a named block
                TempBlock namedTempBlock2;
                if (namedTempBlocks.TryGetValue(blockDataValue.Name, out namedTempBlock2))
                {
                    // named block

                    var isTopMost = namedBlockDataValues.ContainsValue(blockDataValue);

                    if (!isTopMost)
                    {
                        if (!string.IsNullOrWhiteSpace(blockDataValue.Source))
                            throw new Exception("Only the top-most named block can define a source.");
                        if (!string.IsNullOrWhiteSpace(blockDataValue.Type))
                            throw new Exception("Only the top-most named block can define a type.");
                        if (!string.IsNullOrWhiteSpace(blockDataValue.DataJson))
                            throw new Exception("Only the top-most named block can define some data.");
                        if (!string.IsNullOrWhiteSpace(blockDataValue.FragmentJson))
                            throw new Exception("Only the top-most named block can define a fragment.");
                    }

                    // add our own blocks here
                    if (!namedTempBlock2.Locked)
                        namedTempBlock2.BlockDatas.AddRange(blockDataValue.Blocks.Select(x => new WithLevel<BlockDataValue>(x, blockLevel)));

                    // if reset, lock temp block blocks collection
                    if (blockDataValue.IsReset)
                        namedTempBlock2.Locked = true;

                    // if kill, kill temp block so it's not inserted
                    if (blockDataValue.IsKill)
                        namedTempBlock2.Killed = namedTempBlock2.Locked = true;

                    // insert temp block at the position of the top-most occurence
                    if (isTopMost && !namedTempBlock2.Killed)
                        tempBlocks.Add(namedTempBlock2);
                }
                else
                {
                    // not a named block
                    
                    // just add a new temp block
                    tempBlocks.Add(new TempBlock
                    {
                        Name = blockDataValue.Name,
                        Source = blockDataValue.Source,
                        DataJson = blockDataValue.DataJson,
                        FragmentJson = blockDataValue.FragmentJson,
                        Blocks = GetTempFromData(block.Item.Blocks, blockLevel) // recurse
                    });
                }

                // fixme must cleanup
                //// check if matches a named block
                //BlockDataValue namedBlockDataValue;
                //if (namedBlockDataValues.TryGetValue(blockDataValue.Name, out namedBlockDataValue))
                //{
                //    // check if the corresponding temp block already exists, else create it
                //    TempBlock namedTempBlock;
                //    if (!namedTempBlocks.TryGetValue(blockDataValue.Name, out namedTempBlock))
                //    {
                //        namedTempBlock = new TempBlock
                //        {
                //            Name = namedBlockDataValue.Name,
                //            Source = namedBlockDataValue.Source,
                //            DataJson = namedBlockDataValue.DataJson,
                //            FragmentJson = namedBlockDataValue.FragmentJson
                //        };
                //        namedTempBlocks.Add(blockDataValue.Name, namedTempBlock);
                //    }

                //    // add our own blocks here
                //    if (!namedTempBlock.Locked)
                //        namedTempBlock.BlockDatas.AddRange(blockDataValue.Blocks.Select(x => new WithLevel<BlockDataValue>(x, blockLevel)));

                //    // if reset, lock temp block blocks collection
                //    if (blockDataValue.IsReset)
                //        namedTempBlock.Locked = true;

                //    // if kill, kill temp block so it's not inserted
                //    if (blockDataValue.IsKill)
                //        namedTempBlock.Killed = namedTempBlock.Locked = true;

                //    // insert temp block at the position of the top-most occurence
                //    if (namedBlockDataValue == blockDataValue && !namedTempBlock.Killed)
                //        tempBlocks.Add(namedTempBlock);
                //}
                //else
                //{
                //    // not a named block, just add a new temp block
                //    tempBlocks.Add(new TempBlock
                //    {
                //        Name = blockDataValue.Name,
                //        Source = blockDataValue.Source,
                //        DataJson = blockDataValue.DataJson,
                //        FragmentJson = blockDataValue.FragmentJson,
                //        Blocks = GetTempFromData(block.Item.Blocks, blockLevel) // recurse
                //    });
                //}
            }

            // process named blocks inner blocks
            foreach (var blockTemp in namedTempBlocks.Values)
                blockTemp.Blocks = GetTempFromData(blockTemp.BlockDatas).Reverse().ToArray(); // recurse

            return tempBlocks;
        }

        private static TempBlock[] GetTempFromData(IEnumerable<BlockDataValue> blockDataValues, int level)
        {
            // this is not a named block so its inner blocks do not merge
            // just convert the local blocks - recursively - taking care of levels
            return blockDataValues
                .Where(x => x.MinLevel <= level && x.MaxLevel >= level)
                .Select(blockDataValue =>
                {
                    if (blockDataValue.IsNamed)
                        throw new Exception("Cannot have named blocks under an anonymous block.");

                    return new TempBlock
                    {
                        Name = blockDataValue.Name,
                        Source = blockDataValue.Source,
                        DataJson = blockDataValue.DataJson,
                        FragmentJson = blockDataValue.FragmentJson,
                        Blocks = GetTempFromData(blockDataValue.Blocks, level)
                    };
                }).ToArray();
        }

        private static RenderingStructure GetRenderingFromTemp(TempStructure s)
        {
            var renderingBlocks = s.Blocks.Select(GetRenderingFromTemp).ToArray();
            return new RenderingStructure(s.Source, renderingBlocks);
        }

        private static RenderingBlock GetRenderingFromTemp(TempBlock b)
        {
            var renderingBlocks = b.Blocks.Select(GetRenderingFromTemp).ToArray(); // recurse
            return new RenderingBlock(b.Name, b.Source, renderingBlocks, b.DataJson, b.FragmentJson);
        }
        
        #endregion
    }
}