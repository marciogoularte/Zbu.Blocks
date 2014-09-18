using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace Zbu.Blocks
{
    public class CacheProfile
    {
        internal string Profile { get; set; }

        public int Duration { get; set; }
        public string Mode { get; set; }

        public bool ByPage { get; set; }
        public bool ByMember { get; set; }
        public string ByConst { get; set; }
        public string[] ByQueryString { get; set; }
        public string[] ByProperty { get; set; }

        public string ByCustom { get; set; }

        [JsonIgnore]
        public Func<RenderingBlock, IPublishedContent, ViewDataDictionary, CacheMode> ModeFunc { get; set; }
        [JsonIgnore]
        public Func<HttpRequestBase, RenderingBlock, IPublishedContent, ViewDataDictionary, string> CustomFunc { get; set; }

        internal static readonly Dictionary<string, Func<RenderingBlock, IPublishedContent, ViewDataDictionary, CacheMode>> CacheMode = new Dictionary<string, Func<RenderingBlock, IPublishedContent, ViewDataDictionary, CacheMode>>();
        internal static readonly Dictionary<string, Func<HttpRequestBase, RenderingBlock, IPublishedContent, ViewDataDictionary, string>> CacheCustom = new Dictionary<string, Func<HttpRequestBase, RenderingBlock, IPublishedContent, ViewDataDictionary, string>>();
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
            Mode = cache.Mode;
            ByPage = cache.ByPage;
            ByMember = cache.ByMember;
            ByConst = cache.ByConst;
            ByQueryString = cache.ByQueryString;
            ByProperty = cache.ByProperty;
            ByCustom = cache.ByCustom;

            ModeFunc = cache.ModeFunc;
            CustomFunc = cache.CustomFunc;
        }

        internal CacheMode GetCacheMode(RenderingBlock block, IPublishedContent content, ViewDataDictionary viewData)
        {
            var modeFunc = ModeFunc;
            if (!string.IsNullOrWhiteSpace(Mode) && !CacheMode.TryGetValue(Mode, out modeFunc)) return Blocks.CacheMode.Ignore;
            return modeFunc == null ? Blocks.CacheMode.Ignore : modeFunc(block, content, viewData);
        }

        internal string GetCacheCustom(HttpRequestBase request, RenderingBlock block, IPublishedContent content, ViewDataDictionary viewData)
        {
            var customFunc = CustomFunc;
            if (!string.IsNullOrWhiteSpace(ByCustom) && !CacheCustom.TryGetValue(ByCustom, out customFunc)) return null;
            return customFunc == null ? null : customFunc(request, block, content, viewData);
        }
    }
}
