using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Zbu.Blocks.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Blocks(this HtmlHelper helper, IEnumerable<RenderingBlock> blocks)
        {
            return helper.Blocks(blocks, null);
        }

        public static MvcHtmlString Blocks(this HtmlHelper helper, IEnumerable<RenderingBlock> blocks, object o)
        {
            var viewData = o == null ? null : o.AsViewDataDictionary();

            var s = new StringBuilder();
            foreach (var block in blocks)
                s.Append(helper.Block(block, viewData));
            return new MvcHtmlString(s.ToString());
        }

        public static MvcHtmlString Block(this HtmlHelper helper, RenderingBlock block)
        {
            return helper.Block(block, null);
        }

        public static MvcHtmlString Block(this HtmlHelper helper, RenderingBlock block, object o)
        {
            var viewData = o == null ? null : o.AsViewDataDictionary();

            return helper.Block(block, viewData);
        }

        private static MvcHtmlString Block(this HtmlHelper helper, RenderingBlock block, ViewDataDictionary viewData)
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

        private static ViewDataDictionary AsViewDataDictionary(this object o)
        {
            var viewData = new ViewDataDictionary();
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(o))
            {
                var val = prop.GetValue(o);
                viewData[prop.Name] = val;
            }
            return viewData;
        }
    }
}