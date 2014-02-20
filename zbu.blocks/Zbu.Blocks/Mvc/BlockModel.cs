using System.Globalization;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace Zbu.Blocks.Mvc
{
    public class BlockModel : RenderModel
    {
        public BlockModel(IPublishedContent content, RenderingBlock block)
            : base(content)
        {
            Block = block;
        }

        public BlockModel(IPublishedContent content, RenderingBlock block, CultureInfo culture)
            : base(content, culture)
        {
            Block = block;
        }

        public RenderingBlock Block { get; private set; }
    }
}