using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Umbraco.Core.Models;

namespace Zbu.Blocks.Mvc
{
    public abstract class BlockController
    {
        #region Controller

        private HtmlHelper Helper { get; set; }
        private ControllerContext ControllerContext { get; set; }
        protected ViewDataDictionary ViewData { get; set; }

        public abstract MvcHtmlString Render(IPublishedContent content, RenderingBlock block, CultureInfo currentCulture);

        protected MvcHtmlString Render(string source, BlockModel blockModel)
        {
            MvcHtmlString blockHtml;
            BlocksController controller;

            if (Helper != null)
            {
                controller = Helper.ViewContext.Controller as BlocksController;
                blockHtml = ViewData == null
                    ? Helper.Partial(source, blockModel)
                    : Helper.Partial(source, blockModel, ViewData);
            }
            else if (ControllerContext != null)
            {
                controller = ControllerContext.Controller as BlocksController;
                if (controller == null)
                    throw new Exception("Oops.");

                // this basically repeats what Controller.View is doing

                var viewEngineResult = ViewEngines.Engines.FindView(ControllerContext, source, null);
                if (viewEngineResult == null)
                    throw new Exception("Null ViewEngineResult.");
                var view = viewEngineResult.View;
                if (view == null)
                {
                    // we need to generate an exception containing all the locations we searched
                    // copied over from ViewResult source code
                    var locationsText = new StringBuilder();
                    foreach (var location in viewEngineResult.SearchedLocations)
                    {
                        locationsText.AppendLine();
                        locationsText.Append(location);
                    }
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                        "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}", // MvcResources.Common_ViewNotFound
                        source, locationsText)); 
                }
            
                controller.ViewData.Model = blockModel;

                using (var sw = new StringWriter())
                {
                    var ctx = new ViewContext(ControllerContext, view,
                                              controller.ViewData,
                                              controller.TempData,
                                              sw);
                    view.Render(ctx, sw);
                    blockHtml = new MvcHtmlString(sw.ToString());
                }
            }
            else
                throw new Exception("Oops.");

            var traceBlocksInHtml = controller != null && controller.TraceBlocksInHtml;
            return !traceBlocksInHtml
                ? blockHtml
                : new MvcHtmlString(string.Format("<!-- block:{0} -->{1}{2}{1}<!-- /block:{0} -->{1}",
                    source, Environment.NewLine, blockHtml));
        }

        #endregion

        #region Controllers

        private static readonly Dictionary<string, Type> BlockControllers = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        static BlockController()
        {
            foreach (var type in Umbraco.Core.TypeFinder.FindClassesOfType<BlockController>())
                foreach (var blockSource in type.GetCustomAttributes(typeof (BlockControllerAttribute), false)
                                                .Cast<BlockControllerAttribute>()
                                                .Select(x => x.BlockSource))
                    BlockControllers[blockSource] = type;
        }

        private class DefaultBlockController : BlockController
        {
            public override MvcHtmlString Render(IPublishedContent content, RenderingBlock block, CultureInfo currentCulture)
            {
                // create a block model for the block to render
                // use a basic BlockModel and let the view deal with it
                var blockModel = new BlockModel(content, block, currentCulture);
                return Render(block.Source, blockModel);
            }
        }

        internal static BlockController CreateController(HtmlHelper helper, ViewDataDictionary viewData, string source)
        {
            var c = CreateController(source);
            c.Helper = helper;
            c.ViewData = viewData;
            return c;
        }

        internal static BlockController CreateController(ControllerContext context, string source)
        {
            var c = CreateController(source);
            c.ControllerContext = context;
            return c;
        }

        private static BlockController CreateController(string source)
        {
            Type type;
            var controller = BlockControllers.TryGetValue(source, out type)
                ? Activator.CreateInstance(type) as BlockController
                : new DefaultBlockController();
            if (controller == null)
                throw new Exception(string.Format("Failed to create an instance of class \"{0}\".", type.FullName));
            return controller;
        }

        #endregion
    }
}
