using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Zbu.Blocks.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Block(this HtmlHelper helper, RenderingBlock block)
        {
            // fixme - generic and strongly typed?!
            var m = helper.ViewData.Model as BlockModel;
            if (m == null)
                throw new Exception("bang");
            return helper.Partial(block.Source, new BlockModel(m.Content, block, m.CurrentCulture));
        }
    }
}