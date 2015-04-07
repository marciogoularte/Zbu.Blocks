using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using NUnit.Framework;
using Moq;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Zbu.Blocks.Mvc;

namespace Zbu.Blocks.Tests
{
    [TestFixture]
    public class RenderingTests
    {
        [Test]
        public void RendererViewText()
        {
            // setup environment for unit testing

            RunContext.IsTesting = true;
            RunContext.RuntimeCache = new TestRuntimeCacheProvider();

            // setup MVC

            var controller = new TestBlocksController();

            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Setup(c => c.Controller).Returns(controller);
            var controllerContext = controllerContextMock.Object;

            var engineMock = new Mock<IViewEngine>();
            var engine = engineMock.Object;

            engineMock.Setup(e => e.FindView(It.IsAny<ControllerContext>(), "main", null, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(new MainView(), engine));
            engineMock.Setup(e => e.FindPartialView(It.IsAny<ControllerContext>(), "block1", It.IsAny<bool>()))
                .Returns(new ViewEngineResult(new Block1View(), engine));
            engineMock.Setup(e => e.FindPartialView(It.IsAny<ControllerContext>(), "block2", It.IsAny<bool>()))
                .Returns(new ViewEngineResult(new Block2View(), engine));

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(engine);

            // setup blocks

            BlocksController.Settings.CacheProfiles["forever"] = new CacheProfile
            {
                Mode = "cache",
                Duration = 6666666
            };

            BlocksController.Settings.CacheMode["cache"] = (xBlock, xContent, xViewData) => CacheMode.Cache;

            BlocksController.Settings.MergeMeta = (blockModel, objects) =>
            {
                if (objects == null) return;
                var list = (List<string>) objects["test"];
                if (!blockModel.Meta.ContainsKey("test"))
                    blockModel.Meta["test"] = new List<string>();
                ((List<string>)blockModel.Meta["test"]).AddRange(list);
            };

            // test

            const string json = @"
[
    {
        ""Source"": ""main"",
        ""Blocks"": [
            {
                ""Source"": ""Block1""
            },
            {
                ""Source"": ""Block2"",
                ""Cache"": ""forever""
            }
        ]
    }
]
";

            var content = new PublishedContent
            {
                Structures = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json)
            };
            var model = new RenderModel(content, CultureInfo.CurrentUICulture);

            var rs = RenderingStructure.Compute(null, content, x => ((PublishedContent) x).Structures);
            Assert.IsNotNull(rs);

            var cacheMode = rs.Cache == null
                ? CacheMode.Ignore
                : rs.Cache.GetCacheMode(rs, model.Content, null);

            var text = cacheMode == CacheMode.Ignore
                ? Renderer.ViewText(controllerContext, rs, model.Content, model.CurrentCulture)
                : Renderer.ViewTextWithCache(controllerContext, rs, model.Content, model.CurrentCulture, cacheMode == CacheMode.Refresh);

            Console.WriteLine(text);

            text = cacheMode == CacheMode.Ignore
                ? Renderer.ViewText(controllerContext, rs, model.Content, model.CurrentCulture)
                : Renderer.ViewTextWithCache(controllerContext, rs, model.Content, model.CurrentCulture, cacheMode == CacheMode.Refresh);

            Console.WriteLine(text);

            // expected:
            //main
            //block1 635640043255160266
            //block2 635640043255520520
            //meta: block1,block2
            //main
            //block1 635640043255580629 <<<< different
            //block2 635640043255520520 <<<< identical to previous one
            //meta: block1,block2
        }

        #region Mvc stuff

        public class TestBlocksController : Controller
        { }

        public class ViewDataContainer : IViewDataContainer
        {
            public ViewDataContainer(ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }

            public ViewDataDictionary ViewData { get; set; }
        }

        #endregion

        #region Block controller

        [BlockController("main")]
        public class MainController : BlockControllerBase
        {
            private IPublishedContent _content;

            protected override string Render()
            {
                // ensure we have NO meta data when starting
                if (Meta != null)
                    throw new Exception("Meta is not null?");

                // normal rendering
                // meta data may be added either explicitely or from cache
                var model = new BlockModel(_content, Block, CultureInfo.CurrentUICulture);
                var text = Render(model);

                // report meta data
                // we should see both block1 (explicit) and block2 (cached)
                if (Meta != null)
                    text += "meta: " + string.Join(",", (List<string>)Meta["test"]);

                // in practice we would modify the text using the meta data...

                return text;
            }

            internal override void SetContent(IPublishedContent content)
            {
                _content = content;
            }
        }

        #endregion

        #region Views

        // keep it simple with basic IView instead of Razor views

        public class MainView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                var model = viewContext.ViewData.Model as BlockModel;
                if (model == null) throw new Exception("oops.");

                writer.WriteLine("main");

                foreach (var block in model.Block.Blocks)
                {
                    var helper = new HtmlHelper(viewContext, new ViewDataContainer(viewContext.ViewData));
                    var text = helper.Block(block);
                    writer.Write(text);
                }
            }
        }

        public class Block1View : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                var model = viewContext.ViewData.Model as BlockModel;
                if (model == null) throw new Exception("oops.");

                if (!model.Meta.ContainsKey("test"))
                    model.Meta["test"] = new List<string>();
                ((List<string>)model.Meta["test"]).Add("block1");

                // ticks should change all the time because block1 is not cached
                writer.WriteLine("block1 " + DateTime.Now.Ticks);
            }
        }

        public class Block2View : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                var model = viewContext.ViewData.Model as BlockModel;
                if (model == null) throw new Exception("oops.");

                if (!model.Meta.ContainsKey("test"))
                    model.Meta["test"] = new List<string>();
                ((List<string>)model.Meta["test"]).Add("block2");

                // ticks should NOT change all the time because block2 is cached
                writer.WriteLine("block2 " + DateTime.Now.Ticks);
            }
        }

        #endregion
    }
}
