using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zbu.Blocks
{
    /// <summary>
    /// Provides JSON serialization service.
    /// </summary>
    /// <remarks>See Umbraco's Umbraco.Core.Serialization.JsonNetSerializer</remarks>
    internal class JsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializer()
        {
            _settings = new JsonSerializerSettings();

            // NOTE: no idea, really, this is just a copy of what Core does

            _settings.Converters.Add(new JavaScriptDateTimeConverter());
            _settings.Converters.Add(new EntityKeyMemberConverter());
            _settings.Converters.Add(new KeyValuePairConverter());
            _settings.Converters.Add(new ExpandoObjectConverter());
            _settings.Converters.Add(new XmlNodeConverter());

            _settings.NullValueHandling = NullValueHandling.Include;
            _settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            _settings.TypeNameHandling = TypeNameHandling.Objects;
            //_settings.TypeNameHandling = TypeNameHandling.None;
            _settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
        }

        public string Serialize(object o)
        {
            _settings.TypeNameHandling = TypeNameHandling.None;

            //var s = JsonConvert.SerializeObject(o, Formatting.Indented, _settings);
            var s = JsonConvert.SerializeObject(o, Formatting.None, _settings);
            return s;
        }

        public T Deserialize<T>(string s)
            where T : class
        {
            return JsonConvert.DeserializeObject(s, typeof(T), _settings) as T;
        }
    }
}
