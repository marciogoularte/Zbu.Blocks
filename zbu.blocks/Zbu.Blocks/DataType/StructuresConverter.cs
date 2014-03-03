using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Models.PublishedContent;

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
            var json = source.ToString();
            if (string.IsNullOrWhiteSpace(json)) return null;

            var serializer = new JsonSerializer(); // fixme
            var value = serializer.Deserialize<StructureDataValue[]>(json);
            foreach (var v in value)
                v.EnsureFragments(preview);
            return value;
        }

        public object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            return source;
        }

        public object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview)
        {
            throw new NotImplementedException();
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorGuid == StructuresDataType.DataTypeGuid;
        }
    }
}
