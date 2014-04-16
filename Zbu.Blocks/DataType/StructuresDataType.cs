using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace Zbu.Blocks.DataType
{
    public class StructuresDataType : BaseDataType, IDataType
    {
        public static readonly Guid DataTypeGuid = new Guid("{F2D06C06-0262-463B-B5B9-FD6924CC1545}");

        private IData _data;
        private IDataEditor _editor;
        private IDataPrevalue _prevalue;
        private StructuresPrevalueEditor.ConfigData _config;

        public StructuresPrevalueEditor.ConfigData Config
        {
            get { return _config ?? (_config = ((StructuresPrevalueEditor)PrevalueEditor).Config); }
        }

        public override IData Data
        {
            get { return _data ?? (_data = new DefaultData(this)); }
        }

        public override IDataEditor DataEditor
        {
            get { return _editor ?? (_editor = new StructuresEditor(this)); }
        }

        public override string DataTypeName
        {
            get { return "Structures"; }
        }

        public override Guid Id
        {
            get { return DataTypeGuid; }
        }

        public override IDataPrevalue PrevalueEditor
        {
            get { return _prevalue ?? (_prevalue = new StructuresPrevalueEditor(this)); }
        }
    }
}
