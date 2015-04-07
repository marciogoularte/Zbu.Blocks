using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Zbu.Blocks
{
    // provides an environment so that we can unit test properly

    internal static class RunContext
    {
        private static IRuntimeCacheProvider _runtimeCache;

        public static bool IsTesting { get; set; }

        public static IRuntimeCacheProvider RuntimeCache
        {
            get
            {
                return _runtimeCache ?? ApplicationContext.Current.ApplicationCache.RuntimeCache;
            }
            set
            {
                _runtimeCache = value;
            }
        }

        public static void Reset()
        {
            IsTesting = false;
            _runtimeCache = null;
        }
    }
}
