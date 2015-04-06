using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.Mvc;

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
            var rendered = Renderer.BlockWithCache(helper, block, viewData);
            var text = rendered.Item1;
            var meta = rendered.Item2;

            var model = helper.ViewData.Model as BlockModel;
            if (model == null) throw new Exception("oops.");

            if (BlocksController.Settings.MergeMeta != null && meta != null)
                BlocksController.Settings.MergeMeta(model, meta);

            return new MvcHtmlString(text);
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