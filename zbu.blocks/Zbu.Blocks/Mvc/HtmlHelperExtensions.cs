using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Zbu.Blocks.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Block(this HtmlHelper helper, RenderingBlock block)
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
            return helper.Partial(block.Source, blockModel);
        }
    }
}