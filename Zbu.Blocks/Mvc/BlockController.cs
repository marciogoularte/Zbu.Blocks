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
using Umbraco.Web;

namespace Zbu.Blocks.Mvc
{
    public abstract class BlockController
    {
        #region Controller

        private HtmlHelper Helper { get; set; }
        private ControllerContext ControllerContext { get; set; }
        protected ViewDataDictionary ViewData { get; set; }

        protected RenderingBlock Block { get; private set; }
        protected IPublishedContent Content { get; private set; }
        protected CultureInfo CurrentCulture { get; private set; }
        protected UmbracoHelper Umbraco { get; private set; }

        protected abstract string Render();

        public virtual string RenderText()
        {
            return Render();
        }

        protected string Render(BlockModel blockModel)
        {
            string text;
            BlocksController controller;

            if (Helper != null)
            {
                controller = Helper.ViewContext.Controller as BlocksController;
                text = ViewData == null
                    ? Helper.Partial(Block.Source, blockModel).ToString()
                    : Helper.Partial(Block.Source, blockModel, ViewData).ToString();
            }
            else if (ControllerContext != null)
            {
                controller = ControllerContext.Controller as BlocksController;
                if (controller == null)
                    throw new Exception("Oops.");

                // this basically repeats what Controller.View is doing

                var viewEngineResult = ViewEngines.Engines.FindView(ControllerContext, Block.Source, null);
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
                        Block.Source, locationsText)); 
                }
            
                controller.ViewData.Model = blockModel;

                using (var sw = new StringWriter())
                {
                    var ctx = new ViewContext(ControllerContext, view,
                                              controller.ViewData,
                                              controller.TempData,
                                              sw);
                    view.Render(ctx, sw);
                    text = sw.ToString();
                }
            }
            else
                throw new Exception("Oops.");

            var traceBlocksInHtml = controller != null && controller.TraceBlocksInHtml;
            return !traceBlocksInHtml
                ? text
                : string.Format("<!-- block:{0} -->{1}{2}{1}<!-- /block:{0} -->{1}",
                    Block.Source, Environment.NewLine, text);
        }

        #endregion

        #region Controllers

        private static readonly Dictionary<string, Type> BlockControllers = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        static BlockController()
        {
            foreach (var type in global::Umbraco.Core.TypeFinder.FindClassesOfType<BlockController>())
                foreach (var blockSource in type.GetCustomAttributes(typeof (BlockControllerAttribute), false)
                                                .Cast<BlockControllerAttribute>()
                                                .Select(x => x.BlockSource))
                    BlockControllers[blockSource] = type;
        }

        private class DefaultBlockController : BlockController
        {
            protected override string Render()
            {
                // create a block model for the block to render
                // use a basic BlockModel and let the view deal with it
                var blockModel = new BlockModel(Content, Block, CurrentCulture);
                return Render(blockModel);
            }
        }

        internal static BlockController CreateController(HtmlHelper helper, RenderingBlock block, IPublishedContent content, CultureInfo currentCulture, ViewDataDictionary viewData)
        {
            var c = CreateController(block, content, currentCulture);
            c.Helper = helper;
            c.ViewData = viewData;
            return c;
        }

        internal static BlockController CreateController(ControllerContext context, RenderingBlock block, IPublishedContent content, CultureInfo currentCulture)
        {
            var c = CreateController(block, content, currentCulture);
            c.ControllerContext = context;
            return c;
        }

        private static BlockController CreateController(RenderingBlock block, IPublishedContent content, CultureInfo currentCulture)
        {
            Type type;
            var controller = BlockControllers.TryGetValue(block.Source, out type)
                ? Activator.CreateInstance(type) as BlockController
                : new DefaultBlockController();
            if (controller == null)
                throw new Exception(string.Format("Failed to create an instance of class \"{0}\".", type.FullName));

            controller.Block = block;
            controller.Content = content;
            controller.CurrentCulture = currentCulture;
            controller.Umbraco = new UmbracoHelper(UmbracoContext.Current, content);

            return controller;
        }

        #endregion
    }
}
