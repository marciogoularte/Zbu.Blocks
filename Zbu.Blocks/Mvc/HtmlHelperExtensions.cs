using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using umbraco;
using umbraco.cms.businesslogic.member;
using umbraco.cms.presentation.create.controls;
using Umbraco.Core;
using Umbraco.Web;

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
            return Renderer.BlockWithCache(helper, block, null);
        }

        public static MvcHtmlString Block(this HtmlHelper helper, RenderingBlock block, object o)
        {
            var viewData = o == null ? null : o.AsViewDataDictionary();
            return Renderer.BlockWithCache(helper, block, viewData);
        }

        private static MvcHtmlString Block(this HtmlHelper helper, RenderingBlock block, ViewDataDictionary viewData)
        {
            return Renderer.BlockWithCache(helper, block, viewData);
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