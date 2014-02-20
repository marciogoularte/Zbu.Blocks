using System.Globalization;
using Umbraco.Core.Models;

namespace Zbu.Blocks.Mvc
{
    public class BlockModel<TContent> : BlockModel
        where TContent : class, IPublishedContent
    {
        public BlockModel(TContent content, RenderingBlock block)
            : base(content, block)
        {
            Content = content;
        }

        public BlockModel(TContent content, RenderingBlock block, CultureInfo culture)
            : base(content, block, culture)
        {
            Content = content;
        }

        public new TContent Content { get; private set; }
    }
}