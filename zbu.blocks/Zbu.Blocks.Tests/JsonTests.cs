using System.Collections.Generic;
using NUnit.Framework;

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
                    + "\"Name\":\"testname\","
                    + "\"Source\":\"testsource\","
                    + "\"IsReset\":false,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    + "\"Contexts\":[],"
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
                Data = new Dictionary<string, object> { { "value", 1234} }
            };

            var serializer = new JsonSerializer();
            var json = serializer.Serialize(b);

            Assert.AreEqual(
                "{"
                    + "\"Description\":\"test block\","
                    + "\"Name\":\"testname\","
                    + "\"Type\":\"testtype\","
                    + "\"Source\":\"testsource\","
                    + "\"IsKill\":false,"
                    + "\"IsReset\":true,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    + "\"Data\":{\"value\":1234},"
                    + "\"FragmentType\":null,"
                    + "\"FragmentData\":null,"
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
            Assert.AreEqual("testname", s.Name);
            Assert.AreEqual("testsource", s.Source);
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
                    + "\"Data\":{\"value\":1234},"
                    + "\"FragmentData\":null,"
                    + "\"Blocks\":[]"
                + "}";

            var serializer = new JsonSerializer();
            var b = serializer.Deserialize<BlockDataValue>(json);

            Assert.AreEqual("test block", b.Description);
            Assert.AreEqual("testname", b.Name);
            Assert.AreEqual("testtype", b.Type);
            Assert.AreEqual("testsource", b.Source);
            Assert.IsFalse(b.IsKill);
            Assert.IsTrue(b.IsReset);
            Assert.IsTrue(b.IsNamed);
            Assert.IsNotNull(b.Data);
            Assert.AreEqual(1, b.Data.Count);
            Assert.IsTrue(b.Data.ContainsKey("value"));
            Assert.AreEqual(b.Data["value"], 1234);
            Assert.AreEqual(0, b.Blocks.Length);
        }

        [Test]
        public void DeserializeBlockDataWithFragment()
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
                    + "\"Data\":{\"value\":1234},"
                    + "\"FragmentType\":\"testType\","
                    + "\"FragmentData\":{\"value\":5678},"
                    + "\"Blocks\":[]"
                + "}";

            var serializer = new JsonSerializer();
            var b = serializer.Deserialize<BlockDataValue>(json);

            Assert.AreEqual("test block", b.Description);
            Assert.AreEqual("testname", b.Name);
            Assert.AreEqual("testtype", b.Type);
            Assert.AreEqual("testsource", b.Source);
            Assert.IsFalse(b.IsKill);
            Assert.IsTrue(b.IsReset);
            Assert.IsTrue(b.IsNamed);
            Assert.IsNotNull(b.Data);
            Assert.AreEqual(1, b.Data.Count);
            Assert.IsTrue(b.Data.ContainsKey("value"));
            Assert.AreEqual(b.Data["value"], 1234);
            Assert.AreEqual("testType", b.FragmentType);
            Assert.IsNotNull(b.FragmentData);
            Assert.AreEqual(1, b.FragmentData.Count);
            Assert.IsTrue(b.FragmentData.ContainsKey("value"));
            Assert.AreEqual(b.FragmentData["value"], 5678);
            Assert.AreEqual(0, b.Blocks.Length);
        }

        [Test]
        public void DeserializeBlockDataWithType()
        {
            var json =
                "{"
                    + "\"Description\":\"test block\","
                    + "\"Name\":\"testName\","
                    + "\"Type\":\"foo\","
                    + "\"Source\":\"testSource\","
                    + "\"IsKill\":false,"
                    + "\"IsReset\":true,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    //+ "\"DataJson\":\"\","
                    //+ "\"DataJson\":null,"
                    + "\"FragmentType\":null,"
                    + "\"FragmentData\":null,"
                    + "\"Blocks\":[]"
                + "}";

            BlockDataValue.AddType("foo", new BlockDataValue
            {
                Data = new Dictionary<string, object>{{"doo", 7788}}
            });

            // fixme
            // this works as long as the json does NOT contain DataJson
            // so there's a difference between it being there with a value
            // that can be "" or null, and it NOT being there at all
            // and we should manage that at interface level? how?

            var serializer = new JsonSerializer();
            var b = serializer.Deserialize<BlockDataValue>(json);

            Assert.AreEqual("test block", b.Description);
            Assert.AreEqual("testname", b.Name);
            Assert.AreEqual("foo", b.Type);
            Assert.AreEqual("testsource", b.Source);
            Assert.IsFalse(b.IsKill);
            Assert.IsTrue(b.IsReset);
            Assert.IsTrue(b.IsNamed);
            Assert.IsNotNull(b.Data);
            Assert.AreEqual(1, b.Data.Count);
            Assert.IsTrue(b.Data.ContainsKey("doo"));
            Assert.AreEqual(7788, b.Data["doo"]);
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
            Assert.AreEqual("testsource", s.Source);
            Assert.AreEqual(0, s.Blocks.Length);
        }
    }
}
