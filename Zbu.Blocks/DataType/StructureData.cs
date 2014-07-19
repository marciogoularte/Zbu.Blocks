using System;
using umbraco.cms.businesslogic.datatype;

#if UMBRACO_6
namespace Zbu.Blocks.DataType
{
    // in our case data is just a string and we don't need no custom data

    class StructureData : DefaultData
    {
        public StructureData(BaseDataType dataType)
            : base(dataType)
        {
        }

        public override object Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif