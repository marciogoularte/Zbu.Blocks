using System;
using System.Collections.Generic;
using System.Linq;

namespace Zbu.Blocks
{
    // contains temp. things that should be defined in Umbraco
    // or in their own files - work in progress

    public class PublishedContent
    {
        public PublishedContent Parent { get; set; }

        public StructureData[] Structures { get; set; }
    }

    public class PublishedContentFragment
    { }

    public class Temp
    {
        public RenderingStructure GetRenderingStructure(PublishedContent content, Func<PublishedContent, IEnumerable<StructureData>> propertyAccessor)
        {
            // get the structures that apply to the content
            // for each structure, remember its relative level
            //   so that we can use it to filter the blocks
            // sort order is bottom-top
            var structures = new List<StructureDataWithLevel>();
            var level = 0;
            while (content != null)
            {
                var l = level;
                foreach (var s in propertyAccessor(content)
                    .Where(x => x.MinLevel <= l && x.MaxLevel >= l)
                    .Reverse())
                {
                    var sdwl = new StructureDataWithLevel(s, l);
                    structures.Add(sdwl);
                    if (!s.IsReset) continue;
                    content = null;
                    break;
                }

                level += 1;
                content = content == null ? null : content.Parent;
            }

            // gets the blocks that apply to the content
            // for each block, remember its relative level
            //   so that we can use it to filter the sub-blocks
            // sort order is bottom-top
            var blocks = structures
                .SelectMany(s => s
                    .StructureData.Blocks
                        .Where(x => x.MinLevel <= s.Level && x.MaxLevel >= s.Level)
                        .Reverse()
                        .Select(x => new BlockDataWithLevel(x, s.Level)))
                .ToArray();

            /*
            // find unique blocks
            var uniqueBlockDatas = new Dictionary<string, BlockData>();
            var seen = new HashSet<string>();
            foreach (var blockData in blocks.Reverse().Select(x => x.BlockData)) // top-bottom
            {
                if (blockData.Unique)
                {
                    if (seen.Contains(blockData.SourceKey))
                        throw new Exception("Only topmost can set unique.");
                    uniqueBlockDatas.Add(blockData.SourceKey, blockData);
                }
                seen.Add(blockData.SourceKey);
            }
            seen = null;

            // build the BlockTemps list
            var BlockTemps = new List<BlockTemp>();
            var uniqueBlockTemps = new Dictionary<string, BlockTemp>();
            foreach (var block in blocks)
            {
                var blockData = block.BlockData;
                var blockLevel = block.Level;

                // check if blockData matches a unique block
                BlockData uniqueBlockData;
                if (uniqueBlockDatas.TryGetValue(blockData.SourceKey, out uniqueBlockData))
                {
                    // check if the corresponding BlockTemp already exists, else create it
                    BlockTemp uniqueBlockTemp;
                    if (!uniqueBlockTemps.TryGetValue(blockData.SourceKey, out uniqueBlockTemp))
                    {
                        uniqueBlockTemp = new BlockTemp
                        {
                            Source = uniqueBlockData.Source,
                            Key = uniqueBlockData.Key,
                            BlockJson =  uniqueBlockData.BlockJson
                        };
                        uniqueBlockTemps.Add(blockData.SourceKey, uniqueBlockTemp);
                    }

                    // FIXME handle the sub-blocks
                    // FIXME HANDLE RECURSION ON SUB-UNIQUES
                    // FIXME DIFF BETWEEN COLUMNS & STACK & ??? ONLY DISPLAY, IT'S ALL SUB-AREAS
                    if (!uniqueBlockTemp.Locked)
                        // add our own blocks here
                        //uniqueBlockTemp.Temp.AddRange(uniqueBlockData.Blocks); // fixme - should add the level too
                        uniqueBlockTemp.Temp.AddRange(uniqueBlockData.Blocks.Select(x => new BlockDataWithLevel(x, blockLevel)));

                    // if IsReset, lock BlockTemp blocks collection
                    if (uniqueBlockData.IsReset)
                        uniqueBlockTemp.Locked = true;

                    // if IsKill, kill BlockTemp so it's not inserted
                    if (uniqueBlockData.IsKill)
                        uniqueBlockTemp.Killed = uniqueBlockTemp.Locked = true;

                    // insert BlockTemp at the position of the topmost occurence
                    if (uniqueBlockData == blockData && !uniqueBlockTemp.Killed)
                        BlockTemps.Add(uniqueBlockTemp);
                }
                else
                {
                    // not an unique block, just add a new BlockTemp
                    BlockTemps.Add(new BlockTemp
                    {
                        Source = blockData.Source,
                        Key = blockData.Key,
                        BlockJson = blockData.BlockJson,
                        Blocks = GetTempFromDataNonUniqueBlocks(block.BlockData.Blocks, blockLevel)

                        // FIXME missing a few things here...
                    });
                }
            }

            foreach (var BlockTemp in uniqueBlockTemps)
            {
                uniqueBlockTemps.Blocks = Foo(uniqueBlockTemps.Temp);
            }
            uniqueBlockTemps = null;
            */

            // create the temporary structure
            var structure = new StructureTemp
            {
                Source = structures
                    .Select(x => x.StructureData.Source)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                         ?? "default",
                Blocks = GetTempFromData(blocks).Reverse().ToArray()
            };

            return GetRenderingFromTemp(structure);
        }

        private static BlockTemp[] GetTempFromData(BlockDataWithLevel[] blocks)
        {
            // find unique blocks
            var uniqueBlockDatas = new Dictionary<string, BlockData>();
            var seen = new HashSet<string>();
            foreach (var blockData in blocks.Reverse().Select(x => x.BlockData)) // top-bottom
            {
                if (blockData.IsUnique)
                {
                    // fixme - on recurse?!
                    if (seen.Contains(blockData.SourceKey))
                        throw new Exception("Only topmost can set unique.");
                    uniqueBlockDatas.Add(blockData.SourceKey, blockData);
                }
                seen.Add(blockData.SourceKey);
            }

            // build the BlockTemps list
            var BlockTemps = new List<BlockTemp>();
            var uniqueBlockTemps = new Dictionary<string, BlockTemp>();
            foreach (var block in blocks)
            {
                var blockData = block.BlockData;
                var blockLevel = block.Level;

                // check if blockData matches a unique block
                BlockData uniqueBlockData;
                if (uniqueBlockDatas.TryGetValue(blockData.SourceKey, out uniqueBlockData))
                {
                    // check if the corresponding BlockTemp already exists, else create it
                    BlockTemp uniqueBlockTemp;
                    if (!uniqueBlockTemps.TryGetValue(blockData.SourceKey, out uniqueBlockTemp))
                    {
                        uniqueBlockTemp = new BlockTemp
                        {
                            Source = uniqueBlockData.Source,
                            Key = uniqueBlockData.Key,
                            BlockJson = uniqueBlockData.FragmentJson
                        };
                        uniqueBlockTemps.Add(blockData.SourceKey, uniqueBlockTemp);
                    }

                    // fixme - is that ok?
                    // add our own blocks here
                    if (!uniqueBlockTemp.Locked)
                        uniqueBlockTemp.BlockDatas.AddRange(uniqueBlockData.Blocks.Select(x => new BlockDataWithLevel(x, blockLevel)));

                    // if IsReset, lock BlockTemp blocks collection
                    if (uniqueBlockData.IsReset)
                        uniqueBlockTemp.Locked = true;

                    // if IsKill, kill BlockTemp so it's not inserted
                    if (uniqueBlockData.IsKill)
                        uniqueBlockTemp.Killed = uniqueBlockTemp.Locked = true;

                    // insert BlockTemp at the position of the topmost occurence
                    if (uniqueBlockData == blockData && !uniqueBlockTemp.Killed)
                        BlockTemps.Add(uniqueBlockTemp);
                }
                else
                {
                    // not an unique block, just add a new BlockTemp
                    BlockTemps.Add(new BlockTemp
                    {
                        Source = blockData.Source,
                        Key = blockData.Key,
                        BlockJson = blockData.FragmentJson,
                        Blocks = GetTempFromDataNonUniqueBlocks(block.BlockData.Blocks, blockLevel)

                        // FIXME missing a few things here...
                    });
                }
            }

            foreach (var BlockTemp in uniqueBlockTemps.Values)
            {
                BlockTemp.Blocks = GetTempFromData(BlockTemp.BlockDatas.ToArray());
            }

            return BlockTemps.ToArray();
        }

        private static BlockTemp[] GetTempFromDataNonUniqueBlocks(IEnumerable<BlockData> blockDatas, int level)
        {
            // this is not a unique block so its inner blocks do not merge
            // just convert the local blocks - recursively - taking care of levels
            return blockDatas
                .Where(x => x.MinLevel <= level && x.MaxLevel >= level)
                .Select(blockData =>
                {
                    if (blockData.IsUnique)
                        throw new Exception("Cannot have unique under non-unique.");

                    return new BlockTemp
                    {
                        Source = blockData.Source,
                        Key = blockData.Key,
                        BlockJson = blockData.FragmentJson,
                        Blocks = GetTempFromDataNonUniqueBlocks(blockData.Blocks, level)
                    };
                }).ToArray();
        }

        private static RenderingStructure GetRenderingFromTemp(StructureTemp s)
        {
            var blocks = s.Blocks.Select(GetRenderingFromTemp).ToArray();
            return new RenderingStructure(s.Source, blocks);
        }

        private static RenderingBlock GetRenderingFromTemp(BlockTemp b)
        {
            var blocks = b.Blocks.Select(GetRenderingFromTemp).ToArray(); // recurse
            return new RenderingBlock(b.Source, b.Key, blocks, null); // fixme fragment?!
        }
    }
}
