using System;
using NUnit.Framework;

namespace Zbu.Blocks.Tests
{
    [TestFixture]
    public class RenderingStructureTests
    {
        [Test]
        public void SingleLevelSingleStructure()
        {
            // test that one basic structure can be processed

            const string json = "["
                + "{"
                    + "\"Source\":\"test\","
                    + "\"Name\":\"\","
                    + "\"IsReset\":false,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":666,"
                    + "\"Blocks\":[]"
                + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void SingleLevelDualStructures()
        {
            // test structures order when two structures exist
            // test2 should be picked

            const string json = "["
                + "{"
                    + "\"Source\":\"test1\""
                + "},"
                + "{"
                    + "\"Source\":\"test2\""
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void SingleLevelDualStructuresWithLevel()
        {
            // test structure exclusion by level
            // same as test above, but test2 is ignored

            const string json = "["
                + "{"
                    + "\"Source\":\"test1\""
                + "},"
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"MinLevel\":8"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test1", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void MultipleLevelSingleStructure()
        {
            // test that one basic structure can be processed

            const string json = "["
                + "{"
                    + "\"Source\":\"test\","
                    + "\"Name\":\"\","
                    + "\"IsReset\":false,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":666,"
                    + "\"Blocks\":[]"
                + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json),
                Parent = new PublishedContent()
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void MultipleLevelSingleStructureOnParent()
        {
            // test that one basic structure, defined on parent, can be processed

            const string json = "["
                + "{"
                    + "\"Source\":\"test\","
                    + "\"Name\":\"\","
                    + "\"IsReset\":false,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":666,"
                    + "\"Blocks\":[]"
                + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void MultipleLevelDualStructures()
        {
            // test that structures at various levels can be processed

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\""
                + "}"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\""
                + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void MultipleLevelDualStructuresWithLevel()
        {
            // test that levels work with parents

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"MinLevel\":1,"
                    + "\"MaxLevel\":1"
                + "}"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"MinLevel\":8"
                + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test1", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void OneStructureWithBlocks()
        {
            // ensure it works

            const string json = "["
                + "{"
                    + "\"Source\":\"test\","
                    + "\"Name\":\"\","
                    + "\"IsReset\":false,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":666,"
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b1\","
                        + "},"
                        + "{"
                            + "\"Source\":\"b2\","
                        + "},"
                    + "]"
                + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test", s.Source);
            Assert.AreEqual(2, s.Blocks.Length);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresWithBlocks()
        {
            // ensure blocks are in the right order

            const string json = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b1\","
                        + "},"
                    + "]"
                + "},"
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b2\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(2, s.Blocks.Length);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocks()
        {
            // ensure blocks are in the right order

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b1\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b2\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(2, s.Blocks.Length);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocksNonUnique()
        {
            // ensure blocks are in the right order

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"Name\":\"b1\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"Name\":\"b2\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(2, s.Blocks.Length);
            Assert.AreEqual("b", s.Blocks[0].Source);
            Assert.AreEqual("b", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocksUnique()
        {
            // ensure blocks are in the right order

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            +"\"IsUnique\":true,"
                            + "\"Name\":\"b1\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"Name\":\"b2\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(1, s.Blocks.Length);
            Assert.AreEqual("b", s.Blocks[0].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocksInvalidUnique1()
        {
            // cannot set IsUnique on block other than top-most

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"Name\":\"b1\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"IsUnique\":true,"
                            + "\"Name\":\"b2\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            Assert.Throws<Exception>(() => RenderingStructure.Compute(p, x => x.Structures));
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocksInvalidUnique2()
        {
            // cannot set isUnique twice

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"IsUnique\":true,"
                            + "\"Name\":\"b1\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"IsUnique\":true,"
                            + "\"Name\":\"b2\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            Assert.Throws<Exception>(() => RenderingStructure.Compute(p, x => x.Structures));
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocksAndReset()
        {
            // ensure reset works
            // because the structure resets, the entire structures above it are ignored

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b1\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"IsReset\":true,"
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b2\","
                        + "},"
                    + "]"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("default", s.Source);
            Assert.AreEqual(1, s.Blocks.Length);
            Assert.AreEqual("b2", s.Blocks[0].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocksAndMinLevel()
        {
            // ensure blocks are in the right order

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b1before\","
                            + "\"MinLevel\":8"
                        + "},"
                        + "{"
                            + "\"Source\":\"b1\","
                        + "},"
                        + "{"
                            + "\"Source\":\"b1before\","
                            + "\"MinLevel\":8"
                        + "},"
                    + "]"
                + "},"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b2before\","
                            + "\"MinLevel\":8"
                        + "},"
                        + "{"
                            + "\"Source\":\"b2\","
                        + "},"
                        + "{"
                            + "\"Source\":\"b2after\","
                            + "\"MinLevel\":8"
                        + "},"
                    + "]"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(2, s.Blocks.Length);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocksUniqueAndSubBlocks()
        {
            // ensure blocks are in the right order

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"IsUnique\":true,"
                            + "\"Blocks\":["
                                + "{"
                                    + "\"Source\":\"b1\","
                                + "},"
                            + "]"
                        + "},"
                    + "]"
                + "},"
                + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"Blocks\":["
                                + "{"
                                    + "\"Source\":\"b2\","
                                + "},"
                            + "]"
                        + "},"
                    + "]"
                + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(1, s.Blocks.Length);
            Assert.AreEqual("b", s.Blocks[0].Source);
            Assert.AreEqual(2, s.Blocks[0].Blocks.Length);
            Assert.AreEqual("b1", s.Blocks[0].Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[0].Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocksUniqueAndSubBlocksAndReset()
        {
            // ensure reset works

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"IsUnique\":true,"
                            + "\"Blocks\":["
                                + "{"
                                    + "\"Source\":\"b1\","
                                + "},"
                            + "]"
                        + "},"
                    + "]"
                + "},"
            + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"b\","
                            + "\"IsReset\":true,"
                            + "\"Blocks\":["
                                + "{"
                                    + "\"Source\":\"b2\","
                                + "},"
                            + "]"
                        + "},"
                    + "]"
                + "},"
            + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(1, s.Blocks.Length);
            Assert.AreEqual("b", s.Blocks[0].Source);
            Assert.AreEqual(1, s.Blocks[0].Blocks.Length);
            Assert.AreEqual("b2", s.Blocks[0].Blocks[0].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocksUniqueAndSubBlocksAndKill()
        {
            // ensure reset works

            const string json1 = "["
                + "{"
                    + "\"Source\":\"test1\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"ba\","
                            + "\"IsUnique\":true,"
                            + "\"Blocks\":["
                                + "{"
                                    + "\"Source\":\"b1\","
                                + "},"
                            + "]"
                        + "},"
                        + "{"
                            + "\"Source\":\"bb\","
                            + "\"IsUnique\":true,"
                            + "\"Blocks\":["
                                + "{"
                                    + "\"Source\":\"b1\","
                                + "},"
                            + "]"
                        + "},"
                    + "]"
                + "},"
            + "]";

            const string json2 = "["
                + "{"
                    + "\"Source\":\"test2\","
                    + "\"Blocks\":["
                        + "{"
                            + "\"Source\":\"ba\","
                            + "\"Blocks\":["
                                + "{"
                                    + "\"Source\":\"b2\","
                                + "},"
                            + "]"
                        + "},"
                        + "{"
                            + "\"Source\":\"bb\","
                            + "\"IsKill\":true,"
                        + "},"
                    + "]"
                + "},"
            + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureData[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(1, s.Blocks.Length);
            Assert.AreEqual("ba", s.Blocks[0].Source);
            Assert.AreEqual(2, s.Blocks[0].Blocks.Length);
            Assert.AreEqual("b1", s.Blocks[0].Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[0].Blocks[1].Source);
        }
    }
}
