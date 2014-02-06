using System;
using System.Collections.Generic;
using System.Linq;

namespace Zbu.Blocks
{
    // contains temp. things that should be defined in Umbraco
    // or in their own files - work in progress

    public class PublishedContent
    {
        public PublishedContent Parent { get; set; }

        public StructureDataValue[] Structures { get; set; }
    }

    public class PublishedContentFragment
    { }
}
