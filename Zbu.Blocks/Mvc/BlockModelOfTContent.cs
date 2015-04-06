using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Zbu.Blocks.Mvc
{
    public class BlockModel<TContent> : BlockModel
        where TContent : class, IPublishedContent
    {
        private readonly BlockModel _model;

        /*
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
        */

        public BlockModel(TContent content, BlockModel model)
            : base(model.Content, model.Block, model.CurrentCulture)
        {
            _model = model;
            Content = content;
        }

        public new TContent Content { get; private set; }

        public override IDictionary<string, object> Meta
        {
            get { return _model.Meta; }
        }
    }
}