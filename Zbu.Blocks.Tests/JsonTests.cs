using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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

            var json = JsonSerializer.Instance.Serialize(s);

            Assert.AreEqual(
                "{"
                    + "\"Description\":\"test structure\","
                    + "\"Name\":\"testname\","
                    + "\"Source\":\"testsource\","
                    + "\"IsReset\":false,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    + "\"Data\":null,"
                    + "\"Contexts\":[],"
                    + "\"ContentTypes\":[],"
                    + "\"ContentTypesNegate\":false,"
                    + "\"Cache\":null,"
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

            var json = JsonSerializer.Instance.Serialize(b);

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
                    + "\"Index\":" + BlockDataValue.DefaultIndex + ","
                    + "\"Data\":{\"value\":1234},"
                    + "\"FragmentType\":null,"
                    + "\"FragmentData\":null,"
                    + "\"Cache\":null,"
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

            var s = JsonSerializer.Instance.Deserialize<StructureDataValue>(json);

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

            BlockDataValue.ClearTypes();
            BlockDataValue.AddType("testType", new BlockDataValue());

            var b = JsonSerializer.Instance.Deserialize<BlockDataValue>(json);

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

            BlockDataValue.ClearTypes();
            BlockDataValue.AddType("testType", new BlockDataValue());

            var b = JsonSerializer.Instance.Deserialize<BlockDataValue>(json);

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

            BlockDataValue.ClearTypes();
            BlockDataValue.AddType("foo", new BlockDataValue
            {
                Data = new Dictionary<string, object>{{"doo", 7788}}
            });

            var b = JsonSerializer.Instance.Deserialize<BlockDataValue>(json);

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

            var s = JsonSerializer.Instance.Deserialize<StructureDataValue>(json);

            Assert.AreEqual(string.Empty, s.Name);
            Assert.AreEqual("testsource", s.Source);
            Assert.AreEqual(0, s.Blocks.Length);
        }

        [Test]
        public void StripCommentsTrimsSpNlTab()
        {
            const string s = @"

    [ ]

";
            const string e = "[ ]";

            Assert.AreEqual(e, s.Trim());
            var x = JsonSerializer.Instance.DeserializeNoComments<object>(e);
            var y = JsonSerializer.Instance.Deserialize<object>(s);
        }

        [Test]
        public void StripComments()
        {
            const string s = @"

    [
    // single line comments
        { // end of line comments
            ""Foo"":1234,
            ""Url"":""http://this.is.not.a.comment"", // this is a comment
            ""Bah"":""this /* is not a comment */""
        }
/*
    and now some block // comments
    containing a ""string""
*/
    ]

";
            const string e = @"[

        {
            ""Foo"":1234,
            ""Url"":""http://this.is.not.a.comment"",
            ""Bah"":""this /* is not a comment */""
        }

    ]";

            Assert.AreEqual(e, JsonSerializer.StripComments(s));
            var x = JsonSerializer.Instance.DeserializeNoComments<object>(e);
            var y = JsonSerializer.Instance.Deserialize<object>(s);
        }

        [Test]
        public void CustomConverter()
        {
            var json = JsonSerializer.Instance;

            var o1 = new TestObject {Value = "hello", Custom = new CustomObject {Value = "world"}};
            var j1 = json.Serialize(o1);
            Assert.AreEqual("{\"Value\":\"xhello\",\"Custom\":{\"IsSomething\":false,\"Value\":\"world\"}}", j1);
            var o2 = json.Deserialize<TestObject>(j1);
            Assert.AreEqual("hello", o2.Value);
            Assert.IsNotNull(o2.Custom);
            Assert.IsFalse(o2.Custom.IsSomething);
            Assert.AreEqual("world", o2.Custom.Value);

            var o3 = json.Deserialize<TestObject>("{\"Value\":\"xhello\",\"Custom\":null}");
            Assert.IsNull(o3.Custom);
            var o4 = json.Deserialize<TestObject>("{\"Value\":\"xhello\",\"Custom\":\"boom\"}");
            Assert.IsNotNull(o4.Custom);
            Assert.IsTrue(o4.Custom.IsSomething);
            Assert.AreEqual("boom", o4.Custom.Value);
        }

        [Test]
        public void Constructor()
        {
            var json = JsonSerializer.Instance;
            var o = json.Deserialize<ObjectWithConstructor>("{\"Value\":\"hello\"}");
            Assert.AreEqual("hello", o.Value);
            Assert.AreEqual("pp", o.Orig);

            o = json.Deserialize<ObjectWithConstructor>("{\"Profile\":\"hello\"}");
            Assert.AreEqual("profiled", o.Value);
            Assert.AreEqual("pp", o.Orig);

            o = json.Deserialize<ObjectWithConstructor>("{\"Profile\":\"hello\",\"Value\":\"hello\"}");
            Assert.AreEqual("hello", o.Value);
            Assert.AreEqual("pp", o.Orig);
        }

        public class TestObject
        {
            [JsonConverter(typeof(CustomStringConverter))]
            public string Value { get; set; }

            [JsonConverter(typeof(CustomObjectConverter))]
            public CustomObject Custom { get; set; }
        }

        public class CustomObject
        {
            public bool IsSomething { get; set; }
            public string Value { get; set; }
        }

        public class ObjectWithConstructor
        {
            public string Value { get; set; }
            public string Orig { get; set; }


            public ObjectWithConstructor()
            {
                Orig = "np";
            }

            [JsonConstructor]
            internal ObjectWithConstructor(string profile)
            {
                Orig = "pp";
                if (profile != null) Value = "profiled";
            }
        }

        public class CustomObjectConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof (CustomObject).IsAssignableFrom(objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Null:
                        return null;
                    case JsonToken.String:
                        return new CustomObject {IsSomething = true, Value = (string)serializer.Deserialize(reader, typeof(string))};
                    default: // just in case...
                    case JsonToken.StartObject:
                        return serializer.Deserialize(reader, objectType);
                }
            }

            public override bool CanWrite
            {
                get { return false; }
            }
        }

        public class CustomStringConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof (string).IsAssignableFrom(objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                serializer.Serialize(writer, "x" + (string)value);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var s = (string)serializer.Deserialize(reader, objectType);
                if (s != null) s = s.Substring(1);
                return s;
            }
        }
    }
}
