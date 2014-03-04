using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;
using Zbu.Blocks.Mvc;

namespace Zbu.Blocks
{
    public class App : ApplicationEventHandler
    {
        // fixme - this should be in w310
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            // register the blocks controller
            BlocksController.Register();

            PublishedContentRequest.Prepared += (sender, args) =>
            {
                // make sure the request has a published content
                var pr = sender as PublishedContentRequest;
                if (pr == null || !pr.HasPublishedContent) return;

                // make sure this is the corporate website
                // and we have proper structures definition
                var sitekey = pr.PublishedContent.GetPropertyValue<string>("sitekey", true);
                if (sitekey != "corporate") return;
                if (!pr.PublishedContent.HasProperty("structures")) return;

                // plug Zbu.Blocks
                // a) because we want to override whatever template is set on the current
                //   content, and route to MVC, and we cannot do that without setting a
                //   template, and we cannot clear the current template, we need to use
                //   a real dummy MVC template
                // b) tell the blocks controller that it needs to handle the request
                pr.TrySetTemplate("dummy-mvc");
                BlocksController.HandleThisRequest();
            };
        }
    }
}
