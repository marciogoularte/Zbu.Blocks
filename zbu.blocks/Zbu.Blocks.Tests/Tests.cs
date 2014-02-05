using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Zbu.Blocks.Tests
{

    // TODO
    // write the JSON parser
    // go to new minLevel, maxLevel... syntax
    // test!
    // split code into files

    // zap raw data after conversion?
    // then handle fragments

    [TestFixture]
    public class Tests
    {
        [Test]
        public void SingleLevelSingleStructure()
        {
            var json = "["
                + "{"
                    + "\"Source\":\"test\","
                    + "\"Name\":\"\","
                    + "\"IsReset\":false,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    + "\"Blocks\":[]"
                + "}"
                + "]";

            var serializer = new JsonSerializer();

            var p = new PublishedContent
            {
                Structures = serializer.Deserialize<StructureData[]>(json)
            };

            var temp = new Temp();
            var s = temp.GetRenderingStructure(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void SingleLevelDualStructures()
        {
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

            var temp = new Temp();
            var s = temp.GetRenderingStructure(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test2", s.Source);
            Assert.IsEmpty(s.Blocks);
        }

        [Test]
        public void SingleLevelDualStructuresWithLevel()
        {
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

            var temp = new Temp();
            var s = temp.GetRenderingStructure(p, x => x.Structures);

            Assert.IsNotNull(s);
            Assert.AreEqual("test1", s.Source);
            Assert.IsEmpty(s.Blocks);
        }
    }
}
