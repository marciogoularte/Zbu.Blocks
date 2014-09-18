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
        /// <param name="cache">The cache paramters.</param>
        /// <remarks>The structure data can be null.</remarks>
        public RenderingStructure(string source, IEnumerable<RenderingBlock> blocks, IDictionary<string, object> data, CacheProfile cache)
            : base(null, source, blocks, data, null, cache)
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

        private static bool AppliesByContext(StructureDataValue structure, string context)
        {
            // todo - would we want to support negated contexts?
            // ie do NOT apply do 'ajax-*'? but to everything else?

            // null context => applies if has no contexts or contexts contains 'null'
            if (context == null)
                return structure.Contexts.Length == 0 || structure.Contexts.Contains(null);

            // non-null context => cannot apply if contexts is empty
            if (structure.Contexts.Length == 0)
                return false;

            // applies if contexts contains the context
            if (structure.Contexts.Contains(context, StringComparer.InvariantCultureIgnoreCase))
                return true;

            // applies if a contexts item is a wildcard and matches
            return structure.Contexts.Any(x =>
                x != null
                && x.EndsWith("*")
                && context.StartsWith(x.Substring(0, x.Length - 1), StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool AppliesByContentType(StructureDataValue structure, string contentType)
        {
            // note: do not have to handle 'null' here as we do with contexts, because the required
            // context could be null (no context) but the current content type is never null.

            if (structure.ContentTypes == null || structure.ContentTypes.Length == 0)
                return true;

            var contains = structure.ContentTypes.Contains(contentType, StringComparer.InvariantCultureIgnoreCase);
            return structure.ContentTypesNegate ? !contains : contains;
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
                    .Where(x => x.MinLevel <= l && x.MaxLevel >= l
                        && AppliesByContext(x, checkContext)
                        && AppliesByContentType(x, baseContentTypeAlias))
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

            // walk up the structures looking for the first non-null cache info
            var wcache = structureDataValues.FirstOrDefault(x => x.Item.Cache != null);
            var cache = wcache == null ? null : wcache.Item.Cache;

            // recursively get the blocks
            var blocks = GetTempFromData(blockDataValues); // bottom-up -> top-bottom

            // merge all data
            var data = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            structureDataValues.Reverse(); // top-bottom
            foreach (var structureDataValue in structureDataValues.Where(x => x.Item.Data != null))
                foreach (var kvp in structureDataValue.Item.Data)
                    data[kvp.Key] = kvp.Value;

            return new RenderingStructure(source, blocks.Select(GetRenderingFromTemp), data.Count == 0 ? null : data, cache);
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
            // the name is case-insensitive
            var namedBlockDataValueDictionary = new Dictionary<string, List<WithLevel<BlockDataValue>>>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var b in blockDataValuesArray.Where(x => x.Item.IsNamed))
            {
                List<WithLevel<BlockDataValue>> namedBlockDataValues;
                if (!namedBlockDataValueDictionary.TryGetValue(b.Item.Name, out namedBlockDataValues)) 
                    namedBlockDataValues = namedBlockDataValueDictionary[b.Item.Name] = new List<WithLevel<BlockDataValue>>();
                namedBlockDataValues.Add(b);
            }
            foreach (var kvp in namedBlockDataValueDictionary)
                kvp.Value.Reverse(); // top-to-bottom

            // create the temporary named blocks from the top-most block data value
            // whether bottom named blocks can change any of these settings is an option
            var namedTempBlocks = new Dictionary<string, TempBlock>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var namedBlockDataValues in namedBlockDataValueDictionary.Values)
            {
                var blockDataValue = namedBlockDataValues[0].Item;
                var blockLevel = namedBlockDataValues[0].Level;

                var t = new TempBlock
                {
                    Name = blockDataValue.Name,
                    Source = blockDataValue.Source,
                    Index = blockDataValue.Index,
                    Fragment = blockDataValue.Fragment,
                    Cache = blockDataValue.Cache
                };
                t.MergeData(blockDataValue.Data);
                namedTempBlocks[blockDataValue.Name] = t;

                t.BlockDataValues.AddRange(blockDataValue.Blocks
                    .Where(b => b.MinLevel <= blockLevel && b.MaxLevel >= blockLevel)
                    .Select(x => new WithLevel<BlockDataValue>(x, blockLevel)));

                foreach (var namedOtherDataValue in namedBlockDataValues.Skip(1)) // top-bottom
                {
                    var otherDataValue = namedOtherDataValue.Item;
                    var otherLevel = namedOtherDataValue.Level;

                    if (otherDataValue.IsKill)
                    {
                        t.Killed = true;
                        break; // no need to continue
                    }

                    // set source if not already set
                    if (!string.IsNullOrWhiteSpace(otherDataValue.Source))
                    {
                        if (!string.IsNullOrWhiteSpace(t.Source))
                            throw new StructureException("Cannot change source of a named blocks once it has been set.");
                        t.Source = otherDataValue.Source;
                    }

                    // index cannot change
                    if (otherDataValue.Index != BlockDataValue.DefaultIndex)
                        throw new StructureException("Only the top-most named block can define an index.");

                    // merge data
                    t.MergeData(otherDataValue.Data);

                    // override fragment
                    if (otherDataValue.Fragment != null)
                        t.Fragment = otherDataValue.Fragment;

                    // set cache if not already set
                    if (otherDataValue.Cache != null)
                    {
                        if (t.Cache != null)
                            throw new StructureException("Cannot change cache of a named blocks once it has been set.");
                        t.Cache = otherDataValue.Cache;
                    }

                    if (otherDataValue.IsReset)
                        t.BlockDataValues.Clear();

                    t.BlockDataValues.AddRange(otherDataValue.Blocks
                        .Where(b => b.MinLevel <= otherLevel && b.MaxLevel >= otherLevel)
                        .Select(x => new WithLevel<BlockDataValue>(x, otherLevel)));
                }

                t.BlockDataValues.Reverse();
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
                    // insert temp block at the position of the top-most occurence
                    var isTopMost = block == namedBlockDataValueDictionary[blockDataValue.Name][0];
                    if (isTopMost && !namedTempBlock.Killed)
                        tempBlocks.Add(namedTempBlock);
                }
                else
                {
                    // not a named block

                    // just add a new temp block
                    var t = new TempBlock
                    {
                        Name = string.Empty, // not a named block
                        Source = blockDataValue.Source,
                        Index = blockDataValue.Index,
                        Fragment = blockDataValue.Fragment,
                        Cache = blockDataValue.Cache,

                        // there's nothing to merge so all blocks are defined at the same level
                        // block.Item.Blocks is top-bottom, must reverse
                        Blocks = GetTempFromData(block.Item.Blocks.Reverse()
                            .Where(b => b.MinLevel <= blockLevel && b.MaxLevel >= blockLevel)
                            .Select(b => new WithLevel<BlockDataValue>(b, blockLevel)))
                    };
                    t.MergeData(blockDataValue.Data);
                    tempBlocks.Add(t);
                }
            }

            // process named blocks source & inner blocks
            foreach (var tempBlock in namedTempBlocks.Values)
            {
                // for a named block, source may be missing and then we use the name as a source
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
            return new RenderingBlock(b.Name, b.Source, renderingBlocks, b.Data, b.Fragment, b.Cache);
        }

        #endregion
    }
}