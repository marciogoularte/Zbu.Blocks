﻿using NUnit.Framework;

namespace Zbu.Blocks.Tests
{
    [TestFixture]
    public class JsonTests
    {
        [Test]
        public void SerializeStructureData()
        {
            var s = new StructureDataValue
            {
                Description = "test structure",
                Name = "testName",
                Source = "testSource"
            };

            var serializer = new JsonSerializer();
            var json = serializer.Serialize(s);

            Assert.AreEqual(
                "{"
                    + "\"Description\":\"test structure\","
                    + "\"Name\":\"testName\","
                    + "\"Source\":\"testSource\","
                    + "\"IsReset\":false,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    + "\"Blocks\":[]"
                + "}", json);
        }

        [Test]
        public void SerializeBlockData()
        {
            var b = new BlockDataValue
            {
                Description = "test block",
                Name = "testName",
                Type = "testType",
                Source = "testSource",
                IsReset = true,
                DataJson = "datadata"
            };

            var serializer = new JsonSerializer();
            var json = serializer.Serialize(b);

            Assert.AreEqual(
                "{"
                    + "\"Description\":\"test block\","
                    + "\"Name\":\"testName\","
                    + "\"Type\":\"testType\","
                    + "\"Source\":\"testSource\","
                    + "\"IsKill\":false,"
                    + "\"IsReset\":true,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    + "\"DataJson\":\"datadata\","
                    + "\"FragmentJson\":\"\","
                    + "\"Blocks\":[]"
                + "}", json);
        }

        [Test]
        public void DeserializeStructureData()
        {
            var json = 
                "{"
                    + "\"Description\":\"test structure\","
                    + "\"Name\":\"testName\","
                    + "\"Source\":\"testSource\","
                    + "\"IsReset\":false,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    + "\"Blocks\":[]"
                + "}";

            var serializer = new JsonSerializer();
            var s = serializer.Deserialize<StructureDataValue>(json);

            Assert.AreEqual("test structure", s.Description);
            Assert.AreEqual("testName", s.Name);
            Assert.AreEqual("testSource", s.Source);
            Assert.AreEqual(0, s.Blocks.Length);
        }

        [Test]
        public void DeserializeBlockData()
        {
            var json = 
                "{"
                    + "\"Description\":\"test block\","
                    + "\"Name\":\"testName\","
                    + "\"Type\":\"testType\","
                    + "\"Source\":\"testSource\","
                    + "\"IsKill\":false,"
                    + "\"IsReset\":true,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    + "\"DataJson\":\"datadata\","
                    + "\"FragmentJson\":\"\","
                    + "\"Blocks\":[]"
                + "}";

            var serializer = new JsonSerializer();
            var b = serializer.Deserialize<BlockDataValue>(json);

            Assert.AreEqual("test block", b.Description);
            Assert.AreEqual("testName", b.Name);
            Assert.AreEqual("testType", b.Type);
            Assert.AreEqual("testSource", b.Source);
            Assert.IsFalse(b.IsKill);
            Assert.IsTrue(b.IsReset);
            Assert.IsTrue(b.IsNamed);
            Assert.AreEqual("datadata", b.DataJson);
            Assert.AreEqual(0, b.Blocks.Length);
        }

        [Test]
        public void DeserializePartialStructureData()
        {
            const string json = "{"
                + "\"Source\":\"testSource\","
                + "}";

            var serializer = new JsonSerializer();
            var s = serializer.Deserialize<StructureDataValue>(json);

            Assert.AreEqual(string.Empty, s.Name);
            Assert.AreEqual("testSource", s.Source);
            Assert.AreEqual(0, s.Blocks.Length);
        }
    }
}
