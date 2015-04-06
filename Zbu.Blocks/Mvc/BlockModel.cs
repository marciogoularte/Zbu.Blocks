using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace Zbu.Blocks.Mvc
{
    // register that BlockModelTypeConverter can convert BlockModel to something else
    // this is used to convert to BlockModel<TContent> in views
    [TypeConverter(typeof(BlockModelTypeConverter))]
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

        #region Meta

        private IDictionary<string, object> _meta;

        public virtual IDictionary<string, object> Meta
        {
            get { return _meta ?? (_meta = new Dictionary<string, object>()); }
        }

        internal bool HasMeta
        {
            get { return _meta != null; }
        }

        #endregion
    }
}