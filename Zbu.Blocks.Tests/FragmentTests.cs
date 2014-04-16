using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.Web;

namespace Zbu.Blocks.Tests
{
    [TestFixture]
    public class FragmentTests
    {
        [Test]
        [Ignore("Requires a proper UmbracoContext.")]
        public void Test()
        {
            var json =
                "{"
                    + "\"Description\":\"test block\","
                    + "\"Name\":\"testName\","
                    + "\"Type\":\"testType\","
                    + "\"Source\":\"testSource\","
                    + "\"IsKill\":false,"
                    + "\"IsReset\":true,"
                    + "\"MinLevel\":0,"
                    + "\"MaxLevel\":" + int.MaxValue + ","
                    + "\"Data\":{\"value\":1234},"
                    + "\"FragmentType\":\"testType\","
                    + "\"FragmentData\":{\"value\":5678},"
                    + "\"Blocks\":[]"
                + "}";

            var b = JsonSerializer.Instance.Deserialize<BlockDataValue>(json);

            // we need to proper UmbracoContext to move further
            // ie we need content types, content cache... dunno how to do it
            //UmbracoContext.EnsureContext()

            // that one will fail without a proper UmbracoContext
            b.EnsureFragments(false);
        }
    }
}
