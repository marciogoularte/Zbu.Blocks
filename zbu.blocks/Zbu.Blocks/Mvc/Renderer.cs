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
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Core.Security;
using Member = umbraco.cms.businesslogic.member.Member;

namespace Zbu.Blocks.Mvc
{
    class Renderer
    {
        public static MvcHtmlString BlockPartial(HtmlHelper helper, RenderingBlock block, BlockModel blockModel, ViewDataDictionary viewData)
        {
            var blockHtml = viewData == null
                ? helper.Partial(block.Source, blockModel)
                : helper.Partial(block.Source, blockModel, viewData);

            var controller = helper.ViewContext.Controller as BlocksController;
            var traceBlocksInHtml = controller != null && controller.TraceBlocksInHtml;
            return !traceBlocksInHtml
                ? blockHtml
                : new MvcHtmlString(string.Format("<!-- block:{0} -->{1}{2}{1}<!-- /block:{0} -->{1}",
                    block.Source, Environment.NewLine, blockHtml));
        }

        // basic, non-caching version
        public static MvcHtmlString Block(HtmlHelper helper, RenderingBlock block, ViewDataDictionary viewData)
        {
            // nothing?
            if (block == null) return null;

            // get the current block model
            // BlockModel<TContent> : BlockModel
            var currentBlockModel = helper.ViewData.Model as BlockModel;
            if (currentBlockModel == null)
                throw new Exception("Model does not inherit from BlockModel.");

            // create a block model for the block to render
            // use a basic BlockModel and let the view deal with it
            var blockModel = new BlockModel(currentBlockModel.Content, block, currentBlockModel.CurrentCulture);

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

            // render that block
            return Renderer.BlockPartial(helper, block, blockModel, viewData);
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

            // create a block model for the block to render
            // use a basic BlockModel and let the view deal with it
            var blockModel = new BlockModel(currentBlockModel.Content, block, currentBlockModel.CurrentCulture);

            var cachesCookie = umbraco.BusinessLogic.StateHelper.Cookies.Caches["macro"] ?? "cache";

            // bypass cache
            if (cachesCookie == "ignore" 
                || block.Cache == null 
                || block.Cache.GetCacheIf(block, blockModel.Content, viewData) == false)
                return Renderer.BlockPartial(helper, block, blockModel, viewData);

            var key = GetCacheKey(block, currentBlockModel.Content, helper.ViewContext.HttpContext.Request.QueryString, viewData);

            // in order to refresh we have to flush before getting
            if (cachesCookie == "refresh")
                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheByKeySearch(key);

            // render cached
            return (MvcHtmlString)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                key,
                () => Renderer.BlockPartial(helper, block, blockModel, viewData),
                new TimeSpan(0, 0, 0, block.Cache.Duration), // duration
                false, // sliding
                System.Web.Caching.CacheItemPriority.NotRemovable);
        }

        public static ActionResult ViewWithCache(ControllerContext context, RenderingBlock block, BlockModel model, bool refresh)
        {
            var key = GetCacheKey(block, model.Content, context.HttpContext.Request.QueryString, null);

            // in order to refresh we have to flush before getting
            if (refresh)
                ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheByKeySearch(key);

            // render cached
            var text = (string) ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                key,
                () => Renderer.ViewContent(context, block, model),
                new TimeSpan(0, 0, 0, block.Cache.Duration), // duration
                false, // sliding
                System.Web.Caching.CacheItemPriority.NotRemovable);

            var result = new ContentResult { Content = text };
            return result;
        }

        public static string ViewContent(ControllerContext context, RenderingBlock block, BlockModel model)
        {
            var viewEngineResult = ViewEngines.Engines.FindView(context, block.Source, null);
            if (viewEngineResult == null)
                throw new Exception("Null ViewEngineResult.");
            var view = viewEngineResult.View;
            if (view == null)
            {
                // we need to generate an exception containing all the locations we searched
                // copied over from ViewResult source code
                var locationsText = new StringBuilder();
                foreach (var location in viewEngineResult.SearchedLocations)
                {
                    locationsText.AppendLine();
                    locationsText.Append(location);
                }
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}", // MvcResources.Common_ViewNotFound
                    block.Source, locationsText)); 
            }
            
            context.Controller.ViewData.Model = model;

            string text;
            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                          context.Controller.ViewData,
                                          context.Controller.TempData,
                                          sw);
                view.Render(ctx, sw);
                text = sw.ToString();
            }
            return text;
        }

        public static string GetCacheKey(RenderingBlock block, IPublishedContent content, System.Collections.Specialized.NameValueCollection querystring, ViewDataDictionary viewData)
        {
            var key = "Zbu.Blocks__" + block.Source;
            var cache = block.Cache;

            if (cache.ByPage) key += "__p:" + content.Id;
            if (!string.IsNullOrWhiteSpace(cache.ByConst)) key += "__c:" + cache.ByConst;

            if (cache.ByMember)
            {
                // sure it's obsolete but what's the new way of getting the 'current' member ID?
                // currently even v7 HtmlHelperRenderException uses the legacy API
                var member = Member.GetCurrentMember();
                key += "__m:" + (member == null ? 0 : member.Id);
            }

            if (cache.ByQueryString != null)
            {
                key = cache.ByQueryString.Aggregate(key, (current, vb) =>
                    current + "__" + querystring[vb]);
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

            var custom = cache.GetCacheCustom(block, content, viewData);
            if (!string.IsNullOrWhiteSpace(custom))
            {
                key += "__x:" + custom;
            }

            return key.ToLowerInvariant();
        }
    }
}
