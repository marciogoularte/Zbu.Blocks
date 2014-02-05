using NUnit.Framework;

namespace Zbu.Blocks.Tests
{
    [TestFixture]
    public class JsonTests
    {
        [Test]
        public void SerializeStructureData()
        {
            var s = new StructureData
            {
                Source = "test"
            };

            var serializer = new JsonSerializer();
            var json = serializer.Serialize(s);

            Assert.AreEqual("{"
                + "\"Source\":\"test\","
                + "\"Name\":\"\","
                + "\"IsReset\":false,"
                + "\"MinLevel\":0,"
                + "\"MaxLevel\":" + int.MaxValue + ","
                + "\"Blocks\":[]"
                + "}", json);
        }

        [Test]
        public void SerializeBlockData()
        {
            var b = new BlockData
            {
                Source = "test"
            };

            var serializer = new JsonSerializer();
            var json = serializer.Serialize(b);

            Assert.AreEqual("{"
                + "\"Source\":\"test\","
                + "\"Key\":\"\","
                + "\"Name\":\"\","
                + "\"IsUnique\":false,"
                + "\"IsKill\":false,"
                + "\"IsReset\":false,"
                + "\"MinLevel\":0,"
                + "\"MaxLevel\":" + int.MaxValue + ","
                + "\"Blocks\":[],"
                + "\"FragmentJson\":\"\""
                + "}", json);
        }

        [Test]
        public void DeserializeStructureData()
        {
            var json = "{"
                + "\"Source\":\"test\","
                + "\"Name\":\"\","
                + "\"IsReset\":false,"
                + "\"MinLevel\":0,"
                + "\"MaxLevel\":" + int.MaxValue + ","
                + "\"Blocks\":[]"
                + "}";

            var serializer = new JsonSerializer();
            var s = serializer.Deserialize<StructureData>(json);

            Assert.AreEqual("test", s.Source);
            Assert.AreEqual(0, s.Blocks.Length);
        }

        [Test]
        public void DeserializeBlockData()
        {
            var json = "{"
                + "\"Source\":\"test\","
                + "\"Key\":\"kkk\","
                + "\"Name\":\"\","
                + "\"IsUnique\":false,"
                + "\"IsKill\":false,"
                + "\"IsReset\":false,"
                + "\"MinLevel\":0,"
                + "\"MaxLevel\":" + int.MaxValue + ","
                + "\"Blocks\":[],"
                + "\"FragmentJson\":\"\""
                + "}";

            var serializer = new JsonSerializer();
            var b = serializer.Deserialize<BlockData>(json);

            Assert.AreEqual("test", b.Source);
            Assert.AreEqual("test+kkk", b.SourceKey);
            Assert.AreEqual(0, b.Blocks.Length);
        }

        [Test]
        public void DeserializePartialStructureData()
        {
            const string json = "{"
                + "\"Source\":\"test\","
                + "\"Name\":\"\","
                + "}";

            var serializer = new JsonSerializer();
            var s = serializer.Deserialize<StructureData>(json);

            Assert.AreEqual("test", s.Source);
            Assert.AreEqual(0, s.Blocks.Length);
        }
    }
}
