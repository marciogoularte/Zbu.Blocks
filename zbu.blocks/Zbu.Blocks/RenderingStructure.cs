using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Zbu.Blocks
{
    /// <summary>
    /// Represents a structure to be rendered.
    /// </summary>
    public class RenderingStructure : RenderingBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingStructure"/> class with a source and a collection of blocks.
        /// </summary>
        /// <param name="source">The structure source.</param>
        /// <param name="blocks">The structure blocks.</param>
        /// <param name="data">The structure data dictionary (using case-insensitive keys).</param>
        /// <remarks>The structure data can be null.</remarks>
        public RenderingStructure(string source, IEnumerable<RenderingBlock> blocks, IDictionary<string, object> data)
            : base(null, source, blocks, data, null)
        {
            //Source = source;
            //Blocks = new RenderingBlockCollection(blocks);
        }

        /// <summary>
        /// Gets or sets the source of the structure.
        /// </summary>
        /// <remarks>The source determines the view that should be used to render the structure.</remarks>
        //public string Source { get; private set; }

        /// <summary>
        /// Gets or sets the blocks collection of the structure.
        /// </summary>
        //public RenderingBlockCollection Blocks { get; private set; }

        #region Compute

        public static RenderingStructure Compute(IPublishedContent content, Func<IPublishedContent, IEnumerable<StructureDataValue>> propertyAccessor)
        {
            return Compute(null, content, propertyAccessor);
        }

        public static RenderingStructure Compute(string context, IPublishedContent content, Func<IPublishedContent, IEnumerable<StructureDataValue>> propertyAccessor)
        {
            var checkContext = string.IsNullOrWhiteSpace(context) ? null : context.ToLowerInvariant();

            // get the structure data values that apply to the content
            // for each structure, remember its relative level so that we can use it to filter the blocks
            // sort order is bottom-top
            var structureDataValues = new List<WithLevel<StructureDataValue>>();
            var level = 0;
            var baseContentTypeAlias = content.DocumentTypeAlias;
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
                    .Where(x => (checkContext == null && (x.Contexts.Length == 0 || x.Contexts.Contains(null)))
                        || (checkContext != null && x.Contexts.Contains(checkContext, StringComparer.InvariantCultureIgnoreCase)))
                    .Where(x => x.ContentTypes == null || x.ContentTypes.Length == 0
                        || (x.ContentTypesNegate
                            ? !x.ContentTypes.Contains(baseContentTypeAlias, StringComparer.InvariantCultureIgnoreCase)
                            : x.ContentTypes.Contains(baseContentTypeAlias, StringComparer.InvariantCultureIgnoreCase)))
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

            // gets the structure-level block data values that apply to the content
            // for each block, remember its relative level so that we can use it to filter the sub-blocks
            // sort order is bottom-top
            var blockDataValues = structureDataValues // bottom-top
                .SelectMany(s => s
                    .Item.Blocks // top-bottom
                        .Where(x => x.MinLevel <= s.Level && x.MaxLevel >= s.Level)
                        .Reverse() // bottom-top
                        .Select(x => new WithLevel<BlockDataValue>(x, s.Level)));

            // walk up the structures looking for a proper source else fall back to "default"
            var source = structureDataValues
                    .Select(x => string.IsNullOrEmpty(x.Item.Source) ? x.Item.Name : x.Item.Source) // pick .Source else fallback to .Name
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) // pick the first that's not empty
                         ?? "default"; // if everything fails then fall back to "default"

            // recursively get the blocks
            var blocks = GetTempFromData(blockDataValues); // bottom-up -> top-bottom

            // merge all data
            var data = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            structureDataValues.Reverse(); // top-bottom
            foreach (var structureDataValue in structureDataValues.Where(x => x.Item.Data != null))
                foreach (var kvp in structureDataValue.Item.Data)
                    data[kvp.Key] = kvp.Value;

            return new RenderingStructure(source, blocks.Select(GetRenderingFromTemp), data.Count == 0 ? null : data);
        }

        // input is a collection of block data values
        //   defined at structure level or under a named blocks chain
        // output is the resulting collection of temp blocks
        //
        // named blocks are collapsed into one temp block each
        // anonymous blocks each match to one temp block
        //
        // blockDataValues is ordered bottom-top
        // returns blocks ordered top-bottom
        private static IEnumerable<TempBlock> GetTempFromData(IEnumerable<WithLevel<BlockDataValue>> blockDataValues)
        {
            var blockDataValuesArray = blockDataValues.ToArray(); // iterate only once

            // find named blocks
            // because we're iterating bottom-top, this will get the top-most block for each name
            // the name is case-insensitive
            var namedBlockDataValues = new Dictionary<string, BlockDataValue>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var b in blockDataValuesArray.Where(x => x.Item.IsNamed).Select(x => x.Item))
                namedBlockDataValues[b.Name] = b;

            // create the temporary named blocks from the top-most block data value
            // whether bottom named blocks can change any of these settings is an option
            var namedTempBlocks = new Dictionary<string, TempBlock>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var blockDataValue in namedBlockDataValues.Values)
            {
                var t = new TempBlock
                {
                    Name = blockDataValue.Name,
                    Source = blockDataValue.Source,
                    Index = blockDataValue.Index,
                    Fragment = blockDataValue.Fragment
                };
                t.MergeData(blockDataValue.Data);
                namedTempBlocks[blockDataValue.Name] = t;
            }

            // build the temporary blocks list
            var tempBlocks = new List<TempBlock>();
            foreach (var block in blockDataValuesArray) // bottom-top
            {
                var blockDataValue = block.Item;
                var blockLevel = block.Level;

                // check if it matches a named block
                TempBlock namedTempBlock;
                if (namedTempBlocks.TryGetValue(blockDataValue.Name, out namedTempBlock))
                {
                    // named block
                    // don't create another temp block but collapse into the existing one

                    var isTopMost = namedBlockDataValues.ContainsValue(blockDataValue);

                    // non top-most named blocks have some restrictions
                    if (!isTopMost)
                    {
                        // only the top-most named block can set these values
                        if (!string.IsNullOrWhiteSpace(blockDataValue.Source))
                            throw new StructureException("Only the top-most named block can define a source.");
                        if (!string.IsNullOrWhiteSpace(blockDataValue.Type))
                            throw new StructureException("Only the top-most named block can define a type.");
                        if (blockDataValue.Index != BlockDataValue.DefaultIndex)
                            throw new StructureException("Only the top-most named block can set an index.");

                        // merge data - won't do anything if null
                        namedTempBlock.MergeData(blockDataValue.Data);

                        // fully override fragment
                        if (blockDataValue.Fragment != null)
                            namedTempBlock.Fragment = blockDataValue.Fragment;
                    }

                    // add our own blocks to the existing temp block
                    if (!namedTempBlock.Locked)
                        namedTempBlock.BlockDataValues.AddRange(blockDataValue.Blocks.Select(x => new WithLevel<BlockDataValue>(x, blockLevel)));

                    // if reset, lock temp block blocks collection
                    if (blockDataValue.IsReset)
                        namedTempBlock.Locked = true;

                    // if kill, kill temp block so it's not inserted
                    if (blockDataValue.IsKill)
                        namedTempBlock.Killed = namedTempBlock.Locked = true;

                    // insert temp block at the position of the top-most occurence
                    if (isTopMost && !namedTempBlock.Killed)
                        tempBlocks.Add(namedTempBlock);
                }
                else
                {
                    // not a named block

                    // just add a new temp block
                    var t = new TempBlock
                    {
                        Name = blockDataValue.Name,
                        Source = string.IsNullOrWhiteSpace(blockDataValue.Source) ? blockDataValue.Name : blockDataValue.Source,
                        Index = blockDataValue.Index,
                        Fragment = blockDataValue.Fragment,

                        // there's nothing to merge so all blocks are defined at the same level
                        // block.Item.Blocks is top-bottom, must reverse
                        Blocks = GetTempFromData(block.Item.Blocks.Reverse().Select(b => new WithLevel<BlockDataValue>(b, blockLevel)))
                    };
                    t.MergeData(blockDataValue.Data);
                    tempBlocks.Add(t);
                }
            }

            // process named blocks source & inner blocks
            foreach (var tempBlock in namedTempBlocks.Values)
            {
                tempBlock.Source = string.IsNullOrWhiteSpace(tempBlock.Source) ? tempBlock.Name : tempBlock.Source;

                // tempBlock is a named block so tempBlock.BlockDatas is already bottom-top ordered
                tempBlock.Blocks = GetTempFromData(tempBlock.BlockDataValues); // recurse
            }

            tempBlocks.Reverse(); // return top-bottom

            // sort according to indexes
            // beware! List<T>.Sort() is documented as performing an unstable sort
            // whereas Enumerable.OrderBy<TSource, TKey>.Sort() is documented as performing a stable sort,
            // a stable sort meaning that when indexes are equals, the existing order is preserved
            //tempBlocks.Sort((b1, b2) => b1.Index - b2.Index); // NOT!
            tempBlocks = tempBlocks.OrderBy(x => x.Index).ToList();
            return tempBlocks;
        }

        private static RenderingBlock GetRenderingFromTemp(TempBlock b)
        {
            var renderingBlocks = b.Blocks.Select(GetRenderingFromTemp); // recurse
            return new RenderingBlock(b.Name, b.Source, renderingBlocks, b.Data, b.Fragment);
        }

        #endregion
    }
}