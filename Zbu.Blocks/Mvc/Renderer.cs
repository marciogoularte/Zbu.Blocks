using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Security;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Core.Security;
using Member = umbraco.cms.businesslogic.member.Member;

namespace Zbu.Blocks.Mvc
{
    class Renderer
    {
        // basic, non-caching version
        private static MvcHtmlString Block(HtmlHelper helper, RenderingBlock block, ViewDataDictionary viewData)
        {
            // nothing?
            if (block == null) return null;

            // get the current block model
            // BlockModel<TContent> : BlockModel
            var currentBlockModel = helper.ViewData.Model as BlockModel;
            if (currentBlockModel == null)
                throw new Exception("Model does not inherit from BlockModel.");

            var controller = BlockControllerBase.CreateController(helper, block, currentBlockModel.Content, currentBlockModel.CurrentCulture, viewData);
            return new MvcHtmlString(controller.RenderText());

            // we have:
            //   UmbracoViewPage<TModel> : WebViewPage<TModel>
            //     Model is TModel and we know how to map
            //     IPublishedContent, RenderModel, RenderModel<TContent>
            //   UmbracoViewPage : UmbracoViewPage<IPublishedContent>
            //     Model is IPublishedContent
            //
            // and deprecating:
            //   UmbracoTemplatePage<TContent> : UmbracoViewPage<RenderModel<TContent>>
            //     Model is RenderModel<TContent> ie Content is TContent
            //   UmbracoTemplatePage : UmbracoViewPage<RenderModel>
            //     Model is RenderModel ie Content is IPublishedContent

            // UmbracoViewPage<TModel> can map from BlockModel or BlockModel<TContent>
            // because they inherit from RenderModel, and there is no way it can map
            // from anything to BlockModel so that's not an issue.
            // However it does not know how to map BlockModel to BlockModel<TContent>
            // and that is an issue. So it will fallback to using a TypeConverter, which
            // we implemented in BlockModelTypeConverter.
        }

        // caching version
        public static MvcHtmlString BlockWithCache(HtmlHelper helper, RenderingBlock block, ViewDataDictionary viewData)
        {
            // nothing?
            if (block == null) return null;

            // get the current block model
            // BlockModel<TContent> : BlockModel
            var currentBlockModel = helper.ViewData.Model as BlockModel;
            if (currentBlockModel == null)
                throw new Exception("Model does not inherit from BlockModel.");

            var cacheMode = block.Cache == null
                ? CacheMode.Ignore
                : block.Cache.GetCacheMode(block, currentBlockModel.Content, viewData);

            if (block.Cache == null || cacheMode == CacheMode.Ignore) // test null again so ReSharper is happy
                return Block(helper, block, viewData);

            var key = GetCacheKey(block, currentBlockModel.Content, helper.ViewContext.HttpContext.Request, viewData);

            // in order to refresh we have to flush before getting
            if (cacheMode == CacheMode.Refresh)
                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheByKeySearch(key);

            // render cached
            return (MvcHtmlString)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                key,
                () => Block(helper, block, viewData),
                new TimeSpan(0, 0, 0, block.Cache.Duration), // duration
                false, // sliding
                System.Web.Caching.CacheItemPriority.NotRemovable);
        }

        public static string ViewText(ControllerContext context, RenderingBlock block, IPublishedContent content, CultureInfo currentCulture)
        {
            var controller = BlockControllerBase.CreateController(context, block, content, currentCulture);
            return controller.RenderText();
        }

        public static string ViewTextWithCache(ControllerContext context, RenderingBlock block, IPublishedContent content, CultureInfo currentCulture, bool refresh)
        {
            var key = GetCacheKey(block, content, context.HttpContext.Request, null);

            // in order to refresh we have to flush before getting
            if (refresh)
                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheByKeySearch(key);

            // render cached
            var text = (string) ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                key,
                () => ViewText(context, block, content, currentCulture),
                new TimeSpan(0, 0, 0, block.Cache.Duration), // duration
                false, // sliding
                System.Web.Caching.CacheItemPriority.NotRemovable);

            return text;
        }

        private static string GetCacheKey(RenderingBlock block, IPublishedContent content, HttpRequestBase request, ViewDataDictionary viewData)
        {
            var key = "Zbu.Blocks__" + block.Source;
            var cache = block.Cache;

            if (cache.ByPage) key += "__p:" + content.Id;
            if (!string.IsNullOrWhiteSpace(cache.ByConst)) key += "__c:" + cache.ByConst;

            if (cache.ByMember)
            {
                // sure it's obsolete but what's the new way of getting the 'current' member ID?
                // currently even v7 HtmlHelperRenderExtensions uses the legacy API
                var member = Member.GetCurrentMember();
                key += "__m:" + (member == null ? 0 : member.Id);
            }

            if (cache.ByQueryString != null)
            {
                key = cache.ByQueryString.Aggregate(key, (current, vb) =>
                    current + "__" + request.QueryString[vb]);
            }

            if (cache.ByProperty != null)
            {
                key = cache.ByProperty.Aggregate(key, (current, alias) =>
                {
                    var recurse = alias.StartsWith("_");
                    var value = content.GetPropertyValue(recurse ? alias.Substring(1) : alias, recurse);
                    return current + "__v:" + value;
                });
            }

            var custom = cache.GetCacheCustom(request, block, content, viewData);
            if (!string.IsNullOrWhiteSpace(custom))
            {
                key += "__x:" + custom;
            }

            return key.ToLowerInvariant();
        }
    }
}
