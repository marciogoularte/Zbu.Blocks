using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
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
            return DeserializeNoComments<T>(StripComments(s));
        }

        // for unit tests
        internal T DeserializeNoComments<T>(string s)
            where T : class
        {
            return JsonConvert.DeserializeObject(s, typeof(T), _settings) as T;
        }

        //const string ReBlockComments = @"/\*(.*?)\*/";
        //const string ReLineComments = @"//(.*?)\r?\n";
        //const string ReStrings = @"""((\\[^\n]|[^""\n])*)""";
        //const string ReVerbatimStrings = @"@(""[^""]*"")+";

        const string RePattern =
            @"/\*(.*?)\*/" // block comments
            //+ "|" + @"//(.*?)\r?\n" // line comments
            + "|" + @"[\t ]*//(.*?)\r?\n" // line comments
            + "|" + @"""((\\[^\n]|[^""\n])*)""" // strings
            + "|" + @"@(""[^""]*"")+"; // verbatim strings

        // by default JSON does not support comments
        // but we let ppl put comments in theirs
        // so we need to filter them out
        public static string StripComments(string s)
        {
            var r = Regex.Replace(s,
                //ReBlockComments + "|" + ReLineComments + "|" + ReStrings + "|" + ReVerbatimStrings,
                RePattern,
                me =>
                {
                    var tval = me.Value.Trim();
                    if (tval.StartsWith("/*")) return "";
                    if (tval.StartsWith("//")) return Environment.NewLine;
                    //if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                    //    return me.Value.StartsWith("//") ? Environment.NewLine : "";
                    // Keep the literal strings
                    return me.Value;
                },
                RegexOptions.Singleline);
            return r.Trim();
        }

        // no idea how I can access Umbraco's internal serializer
        // so building our own infrastructure here
        public static readonly JsonSerializer Instance = new JsonSerializer();
    }
}
