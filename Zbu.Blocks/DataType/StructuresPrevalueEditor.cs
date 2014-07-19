using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.cms.businesslogic.datatype;
using Umbraco.Core;
using umbraco.interfaces;
using System.Web.UI;
using System.Web.UI.WebControls;

#if UMBRACO_6
namespace Zbu.Blocks.DataType
{
    public class StructuresPrevalueEditor : PlaceHolder, IDataPrevalue
    {
        private readonly BaseDataType _datatype;
        private ConfigData _config;

        //private TextBox _alias;
        //private TextBox _code;

        public StructuresPrevalueEditor(BaseDataType dataType)
        {
            _datatype = dataType;
        }

        public Control Editor
        {
            get { return this; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //_alias = new TextBox
            //{
            //    Text = Config.FragmentTypeAlias
            //};
            //Controls.Add(_alias);

            //_code = new TextBox
            //{
            //   Text = Config.FragmentViewCode, 
            //   TextMode = TextBoxMode.MultiLine,
            //   Rows = 8               
            //};
            //Controls.Add(_code);
        }

        // this is somewhat ugly yet this is how it's done in most built-in editors
        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteLine("<em>Nothing</em>.");
            //writer.WriteLine("<table>");
            //writer.WriteLine("<tr><th>Fragment type alias</th><td>");
            //_alias.RenderControl(writer);
            //writer.Write("</td></tr>");
            //writer.WriteLine("<tr><th>Fragment view code</th><td>");
            //_code.RenderControl(writer);
            //writer.Write("</td></tr>");
            //writer.Write("</table>");
        }

        public void Save()
        {
            _datatype.DBType = DBTypes.Ntext;

            var config = Config;
            //config.FragmentTypeAlias = _alias.Text;
            //config.FragmentViewCode = _code.Text;
            Config = config;
        }

        public class ConfigData
        {
            //public string FragmentTypeAlias;
            //public string FragmentViewCode;
        }

        public ConfigData Config
        {
            get
            {
                if (_config == null)
                {
                    //var id = _datatype.DataTypeDefinitionId;
                    //var prevalues = ApplicationContext.Current.Services.DataTypeService
                    //    .GetPreValuesCollectionByDataTypeId(id)
                    //    .PreValuesAsDictionary;

                    //Umbraco.Core.Models.PreValue prevalue;
                    //var fragmentTypeAlias = prevalues.TryGetValue("FragmentTypeAlias", out prevalue)
                    //    ? prevalue.Value
                    //    : string.Empty;
                    //var fragmentViewCode = prevalues.TryGetValue("FragmentViewCode", out prevalue)
                    //    ? prevalue.Value
                    //    : "@model IPublishedContent\r\n@using Umbraco.Web\r\n\r\n<span>?</span>";

                    _config = new ConfigData
                    {
                        //FragmentTypeAlias = fragmentTypeAlias,
                        //FragmentViewCode = fragmentViewCode
                    };
                }
                return _config;
            }
            set
            {
                var id = _datatype.DataTypeDefinitionId;

                var prevalues = new Dictionary<string, Umbraco.Core.Models.PreValue>();
                //prevalues["FragmentTypeAlias"] = new Umbraco.Core.Models.PreValue(id, value.FragmentTypeAlias);
                //prevalues["FragmentViewCode"] = new Umbraco.Core.Models.PreValue(id, value.FragmentViewCode);

                ApplicationContext.Current.Services.DataTypeService.SavePreValues(id, prevalues);
            }
        }
    }
}
#endif
