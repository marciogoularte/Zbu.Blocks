using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Zbu.Blocks
{
    class CacheProfileConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof (CacheProfile).IsAssignableFrom(objectType);
        }

        public override bool CanWrite
        {
            get { return false; }
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
                    return null; // fixme - or should we actually return a cache object that says please don't cache?
                case JsonToken.String:
                    var profile = (string) serializer.Deserialize(reader, typeof (string));
                    return new CacheProfile(profile);
                default:
                    return serializer.Deserialize(reader, objectType);
            }
        }
    }
}
