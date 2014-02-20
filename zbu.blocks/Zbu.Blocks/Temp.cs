using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;

namespace Zbu.Blocks
{
    public class App : ApplicationEventHandler
    {
        // fixme - in fact that one should be in w310
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            PublishedContentRequest.Prepared += (sender, args) =>
            {
                // make sure the request has a published content
                var pr = sender as PublishedContentRequest;
                if (pr == null || !pr.HasPublishedContent) return;

                // make sure this is the corporate website
                // fixme - nothing to do here so what?
                var sitekey = pr.PublishedContent.GetPropertyValue<string>("sitekey", true);
                if (sitekey != "corporate") return;

                //InitializeRenderingStructure(pr);
                pr.TrySetTemplate("dummy-mvc"); // still need a dummy MVC template 'cos we can't clear it
            };

            // fixme - I want to do it ONLY if we have a proper structure!
            // see controller factory, there has to be a way to pick the right one...
            // for now just test like that
            DefaultRenderMvcControllerResolver.Current.SetDefaultControllerType(typeof(Mvc.BlocksController));
        }
    }
}
