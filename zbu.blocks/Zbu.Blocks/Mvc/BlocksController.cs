using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Zbu.Blocks.Mvc
{
    public class BlocksController : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            var content = model.Content;

            // compute the rendering structure
            var rs = RenderingStructure.Compute(content,
                x => x.GetPropertyValue<IEnumerable<StructureDataValue>>("structures"));
            if (rs == null)
                return base.Index(model);

            // create a basic BlockModel model - if the view wants a generic
            // BlockModel<TContent> then UmbracoViewPage<> will map using the
            // BlockModelTypeConverter.

            // little point creating a strongly typed model here...
            var m = new BlockModel(model.Content, rs, model.CurrentCulture);
            //var m = CreateModel(model.Content, rs, model.CurrentCulture);

            //return CurrentTemplate(m);
            return View(rs.Source, m);
        }

        //private static BlockModel<T> CreateModel<T>(T content, RenderingBlock rs, CultureInfo culture)
        //    where T : class, IPublishedContent
        //{
        //    return new BlockModel<T>(content, rs, culture);
        //}

        public static void Register()
        {
            FilteredControllerFactoriesResolver.Current.InsertType<BlocksControllerFactory>();
        }

        public static void HandleThisRequest()
        {
            BlocksControllerFactory.HandleThisRequest();
        }
    }
}