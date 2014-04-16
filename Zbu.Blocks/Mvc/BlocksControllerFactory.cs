using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Web.Mvc;

namespace Zbu.Blocks.Mvc
{
    public class BlocksControllerFactory : RenderControllerFactory
    {
        private const string ContextItemsKey = "Zbu.Blocks.Mvc.BlocksController";
        //private readonly static string ControllerName = typeof (BlocksController).Name;

        public override bool CanHandle(RequestContext request)
        {
            var x = request.HttpContext.Items[ContextItemsKey];
            return x != null;
        }

        public override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            return typeof (BlocksController);
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            // use our own controller
            return base.CreateController(requestContext, "Blocks");
        }

        internal static void HandleThisRequest()
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null) return;
            httpContext.Items[ContextItemsKey] = true;
        }
    }
}