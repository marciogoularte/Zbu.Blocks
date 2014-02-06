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

            const string json = 
                "["
                    + "{"
                        + "\"Source\":\"test\""
                    + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureDataValue[]>(json)
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

            const string json = 
                "["
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json)
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

            const string json = 
                "["
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json)
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

            const string json = 
                "["
                    + "{"
                        + "\"Source\":\"test\""
                    + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureDataValue[]>(json),
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

            const string json = 
                "["
                    + "{"
                        + "\"Source\":\"test\""
                    + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json)
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

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\""
                    + "}"
                + "]";

            const string json2 = 
                "["
                    + "{"
                        + "\"Source\":\"test2\""
                    + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
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

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"MinLevel\":1,"
                        + "\"MaxLevel\":1"
                    + "}"
                + "]";

            const string json2 = 
                "["
                    + "{"
                        + "\"Source\":\"test2\","
                        + "\"MinLevel\":8"
                    + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
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

            const string json = 
                "["
                    + "{"
                        + "\"Source\":\"test\","
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test", s.Source);
            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresWithBlocks()
        {
            // ensure blocks are in the right order

            const string json = 
                "["
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithBlocks()
        {
            // ensure blocks are in the right order

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Source\":\"b1\","
                            + "},"
                        + "]"
                    + "},"
                + "]";

            const string json2 = 
                "["
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithAnonymousBlocks()
        {
            // ensure blocks are in the right order

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Source\":\"b1\","
                            + "},"
                        + "]"
                    + "},"
                + "]";

            const string json2 = 
                "["
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithNamedBlocks()
        {
            // ensure blocks are in the right order

            const string json1 = 
                "["
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

            const string json2 = 
                "["
                    + "{"
                        + "\"Source\":\"test2\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b1\","
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("b", s.Blocks[0].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithNamedBlocksError()
        {
            // ensure blocks are in the right order

            const string json1 =
                "["
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

            const string json2 =
                "["
                    + "{"
                        + "\"Source\":\"test2\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Source\":\"c\","
                                + "\"Name\":\"b1\","
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("default", s.Source);
            Assert.AreEqual(1, s.Blocks.Count);
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithNamedBlocksAndSubBlocks()
        {
            // ensure blocks are in the right order

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Source\":\"b\","
                                + "\"Name\":\"b\","
                                + "\"Blocks\":["
                                    + "{"
                                        + "\"Source\":\"b1\","
                                    + "},"
                                + "]"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            const string json2 = 
                "["
                    + "{"
                        + "\"Source\":\"test2\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\","
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("b", s.Blocks[0].Source);
            Assert.AreEqual(2, s.Blocks[0].Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[0].Blocks[1].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithNamedBlocksAndSubBlocksAndReset()
        {
            // ensure reset works

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\","
                                + "\"Source\":\"b\","
                                + "\"Blocks\":["
                                    + "{"
                                        + "\"Source\":\"b1\","
                                    + "},"
                                + "]"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            const string json2 =
                "["
                    + "{"
                        + "\"Source\":\"test2\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\","
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
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("b", s.Blocks[0].Source);
            Assert.AreEqual(1, s.Blocks[0].Blocks.Count);
            Assert.AreEqual("b2", s.Blocks[0].Blocks[0].Source);
        }

        [Test]
        public void TwoStructuresTwoLevelsWithNamedBlocksAndSubBlocksAndKill()
        {
            // ensure reset works

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"ba\","
                                + "\"Source\":\"ba\","
                                + "\"Blocks\":["
                                    + "{"
                                        + "\"Source\":\"b1\","
                                    + "},"
                                + "]"
                            + "},"
                            + "{"
                                + "\"Name\":\"bb\","
                                + "\"Blocks\":["
                                    + "{"
                                        + "\"Source\":\"b1\","
                                    + "},"
                                + "]"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            const string json2 = 
                "["
                    + "{"
                        + "\"Source\":\"test2\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"ba\","
                                + "\"Blocks\":["
                                    + "{"
                                        + "\"Source\":\"b2\","
                                    + "},"
                                + "]"
                            + "},"
                            + "{"
                                + "\"Name\":\"bb\","
                                + "\"IsKill\":true,"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = serializer.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("ba", s.Blocks[0].Source);
            Assert.AreEqual(2, s.Blocks[0].Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[0].Blocks[1].Source);
        }
    }
}
