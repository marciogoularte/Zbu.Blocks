using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.Migrations.Syntax.Update;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Zbu.Blocks.Mvc
{
    public class BlocksController : RenderMvcController
    {
        // see http://our.umbraco.org/documentation/Reference/Mvc/custom-controllers
        //
        public override ActionResult Index(RenderModel model)
        {
            var content = model.Content;

            // fixme - should not be here... OR should move the whole stuff in W310
            // fixme - how can we pick the right controller?!
            var sitekey = content.GetPropertyValue<string>("sitekey", true);
            if (sitekey != "corporate") return base.Index(model);

            // compute the rendering structure
            var rs = RenderingStructure.Compute(content,
                x => x.GetPropertyValue<IEnumerable<StructureDataValue>>("structures"));
            if (rs == null)
                return base.Index(model);

            // give strongly-typed a chance
            //var m = new BlockModel(model.Content, rs, model.CurrentCulture);
            var m = CreateModel(model.Content, rs, model.CurrentCulture);

            // no need to use the intermediate 'render' template here
            //return CurrentTemplate(m);
            return View(rs.Source, m);
        }

        private static BlockModel<T> CreateModel<T>(T content, RenderingBlock rs, CultureInfo culture)
            where T : class, IPublishedContent
        {
            return new BlockModel<T>(content, rs, culture);
        }
    }
}