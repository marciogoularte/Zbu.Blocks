using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Models.PublishedContent;
using Zbu.Blocks.PropertyEditors;

namespace Zbu.Blocks.DataType
{
    // note: can cache the converted value at .Content level because it's just
    // a deserialized json and it does not reference anything outside its content.

    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    [PropertyValueType(typeof(IEnumerable<StructureDataValue>))]
    public class StructuresConverter : IPropertyValueConverter
    {
        public object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            // data == source so we can return json to xpath
            return source;
        }

        public object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            // object == deserialized source
            var json = source.ToString();
            if (string.IsNullOrWhiteSpace(json)) return null;

            var value = JsonSerializer.Instance.Deserialize<StructureDataValue[]>(json);
            foreach (var v in value)
                v.EnsureFragments(preview);
            return value;
        }

        public object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview)
        {
            // we don't really want to support XML for blocks
            // so xpath == source == data == the original json
            return source;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
#if UMBRACO_6
            return propertyType.PropertyEditorGuid == StructuresDataType.DataTypeGuid;
#else
            return propertyType.PropertyEditorAlias == StructuresPropertyEditor.StructuresAlias;
#endif
        }
    }
}
