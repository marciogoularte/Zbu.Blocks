using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace Zbu.Blocks
{
    public class CacheProfile
    {
        internal string Profile { get; set; }

        public int Duration { get; set; }
        public string If { get; set; }

        public bool ByPage { get; set; }
        public bool ByMember { get; set; }
        public string ByConst { get; set; }
        public string[] ByQueryString { get; set; }
        public string[] ByProperty { get; set; }

        public string ByCustom { get; set; }

        [JsonIgnore]
        public Func<RenderingBlock, IPublishedContent, ViewDataDictionary, bool> IfFunc { get; set; }
        [JsonIgnore]
        public Func<RenderingBlock, IPublishedContent, ViewDataDictionary, string> CustomFunc { get; set; }

        internal static readonly Dictionary<string, Func<RenderingBlock, IPublishedContent, ViewDataDictionary, bool>> CacheIf = new Dictionary<string, Func<RenderingBlock, IPublishedContent, ViewDataDictionary, bool>>();
        internal static readonly Dictionary<string, Func<RenderingBlock, IPublishedContent, ViewDataDictionary, string>> CacheCustom = new Dictionary<string, Func<RenderingBlock, IPublishedContent, ViewDataDictionary, string>>();
        internal static readonly Dictionary<string, CacheProfile> Profiles = new Dictionary<string, CacheProfile>();

        public CacheProfile()
        { }

        [JsonConstructor]
        internal CacheProfile(string profile)
        {
            if (profile == null) return;
            CacheProfile cache;
            if (!Profiles.TryGetValue(profile, out cache))
                throw new InvalidOperationException(string.Format("\"{0}\" is not a valid profile alias ", profile));
            CopyFromProfile(cache);
        }

        void CopyFromProfile(CacheProfile cache)
        {
            // do not copy profile!
            Duration = cache.Duration;
            If = cache.If;
            ByPage = cache.ByPage;
            ByMember = cache.ByMember;
            ByConst = cache.ByConst;
            ByQueryString = cache.ByQueryString;
            ByProperty = cache.ByProperty;
            ByCustom = cache.ByCustom;

            IfFunc = cache.IfFunc;
            CustomFunc = cache.CustomFunc;
        }

        internal bool GetCacheIf(RenderingBlock block, IPublishedContent content, ViewDataDictionary viewData)
        {
            var ifFunc = IfFunc;
            if (!string.IsNullOrWhiteSpace(If) && !CacheIf.TryGetValue(If, out ifFunc)) return false;
            return ifFunc == null || ifFunc(block, content, viewData);
        }

        internal string GetCacheCustom(RenderingBlock block, IPublishedContent content, ViewDataDictionary viewData)
        {
            var customFunc = CustomFunc;
            if (!string.IsNullOrWhiteSpace(ByCustom) && !CacheCustom.TryGetValue(ByCustom, out customFunc)) return null;
            return customFunc == null ? null : customFunc(block, content, viewData);
        }
    }
}
