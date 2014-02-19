using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.presentation.umbraco.dialogs;
using Umbraco.Web;
using Umbraco.Core.Models;

namespace Zbu.Blocks.Tests
{
    class Temp
    {
        public void Test()
        {
            // making sure it builds
            // should test against real content...

            IPublishedContent p = new PublishedContent();

            // fixme - we should have a structureContext.Alias = 'ajax' etc
            var s = RenderingStructure.Compute(p, x => x.GetPropertyValue<IEnumerable<StructureDataValue>>("structure"));
        }
    }
}
