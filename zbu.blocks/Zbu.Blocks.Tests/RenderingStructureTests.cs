using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Zbu.Blocks.Tests
{
    [TestFixture]
    public class RenderingStructureTests
    {
        [Test]
        public void OneContentOneStructure()
        {
            // test that one basic structure can be processed

            const string json = 
                "["
                    + "{"
                        + "\"Source\":\"test\""
                    + "}"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void OneContentTwoStructures()
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test2", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void OneContentTwoStructuresInContext()
        {
            // without context, test2 is picked
            // with context, only test1 is visble

            const string json =
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"Contexts\": [ \"ctx\" ]"
                    + "},"
                    + "{"
                        + "\"Source\":\"test2\""
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute("ctx", p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);
            Assert.AreEqual("test1", s.Source);

            s = RenderingStructure.Compute(null,  p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);

            s = RenderingStructure.Compute(" ", p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
        }

        [Test]
        public void OneContentTwoStructuresContentTypesIncl()
        {
            const string json =
                "["
                    + "{"
                        + "\"Source\":\"test1\""
                    + "},"
                    + "{"
                        + "\"Source\":\"test2\","
                        + "\"ContentTypes\": [ \"ctype1\" ]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json),
                DocumentTypeAlias = "ctype2"
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);
            Assert.AreEqual("test1", s.Source);

            p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json),
                DocumentTypeAlias = "ctype1"
            };

            s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
        }

        [Test]
        public void OneContentTwoStructuresContentTypesExcl()
        {
            const string json =
                "["
                    + "{"
                        + "\"Source\":\"test1\""
                    + "},"
                    + "{"
                        + "\"Source\":\"test2\","
                        + "\"ContentTypes\": [ \"ctype1\" ],"
                        + "\"ContentTypesNegate\": true"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json),
                DocumentTypeAlias = "ctype2"
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);

            p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json),
                DocumentTypeAlias = "ctype1"
            };

            s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);
            Assert.AreEqual("test1", s.Source);
        }

        [Test]
        public void OneContentTwoStructuresWithLevels()
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test1", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void TwoContentsOneStructureOnChild()
        {
            // test that one basic structure can be processed

            const string json = 
                "["
                    + "{"
                        + "\"Source\":\"test\""
                    + "}"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json),
                Parent = new PublishedContent()
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void TwoContentsOneStructureOnParent()
        {
            // test that one basic structure, defined on parent, can be processed

            const string json = 
                "["
                    + "{"
                        + "\"Source\":\"test\""
                    + "}"
                + "]";

            var p = new PublishedContent
            {
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void TwoContentsTwoStructures()
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test2", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void TwoContentsTwoStructuresWithLevels()
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test", s.Source);

            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void OneContentTwoStructuresWithBlocks()
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test2", s.Source);

            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoContentsTwoStructuresWithBlocks()
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test2", s.Source);

            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoContentsTwoStructuresWithAnonymousBlocks()
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test2", s.Source);

            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoContentsTwoStructuresWithNamedBlocks()
        {
            // ensure blocks are in the right order

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b1\","
                                + "\"Source\":\"b\","
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);

            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("b", s.Blocks[0].Source);
        }

        [Test]
        public void TwoContentsTwoStructuresWithNamedBlocksOverride1()
        {
            // test named blocks can be overriden

            const string json1 =
                "["
                    + "{"
                        + "\"Source\":\"test1\"," // template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\"," // defined a name block
                                + "\"Source\":\"source1\"," // with a source
                                + "\"Data\":{\"x\":123}"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            const string json2 =
                "["
                    + "{"
                        + "\"Source\":\"test2\"," // use another template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\"," // named => same block
                                + "\"Source\":\"source2\"," // override the source
                                + "\"Data\":{\"y\":456}"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            // illegal to override the source
            Assert.Throws<StructureException>(
                () => RenderingStructure.Compute(p, x => ((PublishedContent) x).Structures));
        }

        [Test]
        public void TwoContentsTwoStructuresWithNamedBlocksOverride2()
        {
            // test named blocks can be overriden

            const string json1 =
                "["
                    + "{"
                        + "\"Source\":\"test1\"," // template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\"," // defined a name block
                                + "\"Source\":\"source1\"," // with a source
                                + "\"Data\":{\"x\":123}"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            const string json2 =
                "["
                    + "{"
                        + "\"Source\":\"test2\"," // use another template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\"," // named => same block
                                //+ "\"Source\":\"source2\"," // override the source
                                + "\"Data\":{\"y\":456}"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            // template is ok
            Assert.AreEqual("test2", s.Source);

            // one block
            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("b", s.Blocks[0].Name);

            // source 
            Assert.AreEqual("source1", s.Blocks[0].Source);

            // merged data
            Assert.AreEqual(2, s.Blocks[0].Data.Count);
            Assert.AreEqual(123, s.Blocks[0].Data["x"]);
            Assert.AreEqual(456, s.Blocks[0].Data["y"]);
        }

        [Test]
        public void TwoContentsTwoStructuresWithBlocksAndReset()
        {
            // test structure reset
            // ie entire structures above are ignored

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\"," // template
                        + "\"Blocks\":[" // blocks
                            + "{"
                                + "\"Source\":\"b1\","
                            + "},"
                        + "]"
                    + "},"
                + "]";

            const string json2 = 
                "["
                    + "{"
                        + "\"IsReset\":true," // reset => ignore above
                        + "\"Blocks\":[" // blocks
                            + "{"
                                + "\"Source\":\"b2\","
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            // template is default because top is ignored
            Assert.AreEqual("default", s.Source);

            // only one block because top is ignored
            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("b2", s.Blocks[0].Source);
        }

        [Test]
        public void TwoContentsTwoStructuresWithBlocksAndMinLevel()
        {
            // ensure blocks are in the right order

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\"," // template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Source\":\"b1before\"," // block with minlevel
                                + "\"MinLevel\":8"
                            + "},"
                            + "{"
                                + "\"Source\":\"b1\"," // block
                            + "},"
                            + "{"
                                + "\"Source\":\"b1before\"," // block with minlevel
                                + "\"MinLevel\":8"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            const string json2 = 
                "["
                    + "{"
                        + "\"Source\":\"test2\"," // use another template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Source\":\"b2before\"," // block with minlevel
                                + "\"MinLevel\":8"
                            + "},"
                            + "{"
                                + "\"Source\":\"b2\"," // block
                            + "},"
                            + "{"
                                + "\"Source\":\"b2after\"," // block with minlevel
                                + "\"MinLevel\":8"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            // template is ok
            Assert.AreEqual("test2", s.Source);

            // only blocks with appropriate level are here
            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[1].Source);
        }

        [Test]
        public void TwoContentsTwoStructuresWithNamedBlocksAndSubBlocks()
        {
            // ensure blocks are in the right order

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\"," // template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\"," // define a named block
                                + "\"Source\":\"b\","
                                + "\"Blocks\":[" // define sub-blocks
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
                        + "\"Source\":\"test2\"," // use another template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\"," // named => same block
                                + "\"Blocks\":[" // add sub-blocks
                                    + "{"
                                        + "\"Source\":\"b2\","
                                    + "},"
                                + "]"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            // template is ok
            Assert.AreEqual("test2", s.Source);

            // one block
            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("b", s.Blocks[0].Source);

            // containing two sub-blocks in the right order
            Assert.AreEqual(2, s.Blocks[0].Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[0].Blocks[1].Source);
        }

        [Test]
        public void TwoContentsTwoStructuresWithNamedBlocksAndSubBlocksAndReset()
        {
            // test block reset

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\"," // template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\"," // define a named block
                                + "\"Source\":\"b\","
                                + "\"Blocks\":[" // define sub blocks - will be resetted
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
                        + "\"Source\":\"test2\"," // use another template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"b\"," // named => same block
                                + "\"IsReset\":true," // but reset contents
                                + "\"Blocks\":[" // so this defines the sub blocks
                                    + "{"
                                        + "\"Source\":\"b2\","
                                    + "},"
                                + "]"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            // template is ok
            Assert.AreEqual("test2", s.Source);

            // only one named block
            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("b", s.Blocks[0].Name);
            Assert.AreEqual("b", s.Blocks[0].Source);

            // only one sub-block, b2
            Assert.AreEqual(1, s.Blocks[0].Blocks.Count);
            Assert.AreEqual("b2", s.Blocks[0].Blocks[0].Source);
        }

        [Test]
        public void TwoContentsTwoStructuresWithNamedBlocksAndSubBlocksAndKill()
        {
            // test block kill

            const string json1 = 
                "["
                    + "{"
                        + "\"Source\":\"test1\"," // template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"ba\"," // define a named block
                                + "\"Source\":\"ba\","
                                + "\"Blocks\":[" // define sub-blocks
                                    + "{"
                                        + "\"Source\":\"b1\","
                                    + "},"
                                + "]"
                            + "},"
                            + "{"
                                + "\"Name\":\"bb\"," // define a named block
                                + "\"Blocks\":[" // define sub-blocks
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
                        + "\"Source\":\"test2\"," // use another template
                        + "\"Blocks\":["
                            + "{"
                                + "\"Name\":\"ba\"," // named => same block
                                + "\"Blocks\":[" // add sub-blocks
                                    + "{"
                                        + "\"Source\":\"b2\","
                                    + "},"
                                + "]"
                            + "},"
                            + "{"
                                + "\"Name\":\"bb\"," // named => same block
                                + "\"IsKill\":true," // kill that block
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json2),
                Parent = new PublishedContent
                {
                    Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json1)
                }
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            // template is ok
            Assert.AreEqual("test2", s.Source);

            // only one named block, ba, because bb has been killed
            Assert.AreEqual(1, s.Blocks.Count);
            Assert.AreEqual("ba", s.Blocks[0].Source);

            // block contains the two sub-blocks in the right order
            Assert.AreEqual(2, s.Blocks[0].Blocks.Count);
            Assert.AreEqual("b1", s.Blocks[0].Blocks[0].Source);
            Assert.AreEqual("b2", s.Blocks[0].Blocks[1].Source);
        }

        [Test]
        public void NamedBlockUnderAnonymousBlock()
        {
            const string json =
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"Blocks\":["
                            + "{" // anonymous
                                + "\"Blocks\":["
                                    + "{"
                                        + "\"Name\":\"b\"," // with a named block
                                        + "\"DataJson\":\"{\\\"value\\\":1}\"," // data
                                    + "},"
                                    + "{"
                                        + "\"Source\":\"c\"," // and an anonymous
                                    + "},"
                                    + "{"
                                        + "\"Name\":\"b\"," // repeat
                                        + "\"Data\":{\"value\":2}," // override
                                    + "},"
                                + "]"
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json),
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            // template is ok
            Assert.AreEqual("test1", s.Source);

            // one anonymous block
            Assert.AreEqual(1, s.Blocks.Count);
            var anon = s.Blocks[0];
            Assert.IsNull(s.Blocks[0].Data);

            // only two blocks 'cos one is named, in the right order
            Assert.AreEqual(2, anon.Blocks.Count);
            Assert.AreEqual("b", anon.Blocks[0].Source);
            Assert.AreEqual("c", anon.Blocks[1].Source);

            // and the named block can be accessed via index
            Assert.IsNotNull(anon.Blocks["b"]);

            // and has proper values
            Assert.AreEqual(2, anon.Blocks["b"].Data["value"]);
        }

        [Test]
        public void OneContentManyStructuresAndBlocksAndIndexes1()
        {
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
                                + "\"Index\":10" // move it up
                            + "},"
                        + "]"
                    + "},"
                + "]";

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test2", s.Source);

            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b2", s.Blocks[0].Source);
            Assert.AreEqual("b1", s.Blocks[1].Source);
        }

        [Test]
        public void OneContentManyStructuresAndBlocksAndIndexes2()
        {
            const string json =
                "["
                    + "{"
                        + "\"Source\":\"test1\","
                        + "\"Blocks\":["
                            + "{"
                                + "\"Source\":\"b1\","
                                + "\"Index\":1000" // move it down
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

            var p = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
            };

            var s = RenderingStructure.Compute(p, x => ((PublishedContent)x).Structures);
            Assert.IsNotNull(s);

            Assert.AreEqual("test2", s.Source);

            Assert.AreEqual(2, s.Blocks.Count);
            Assert.AreEqual("b2", s.Blocks[0].Source);
            Assert.AreEqual("b1", s.Blocks[1].Source);
        }
    }
}
