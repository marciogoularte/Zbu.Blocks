using System;
using System.ComponentModel;
using System.Globalization;
using Umbraco.Core;

namespace Zbu.Blocks.Mvc
{
    // converts from BlockModel to BlockModel<TContent>
    // so UmbracoViewPage<BlockModel<TContent>> can work properly
    // see notes in HtmlHelperExtensions
    // BlockModel is marked with the TypeConverterAttribute to use this converter

    public class BlockModelTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            // can convert to BlockModel<TContent>
            return destinationType.IsGenericType
                   && destinationType.GetGenericTypeDefinition() == typeof (BlockModel<>)
                   && destinationType.GetGenericArguments().Length == 1;

            //return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var source = value as BlockModel;
            if (source == null)
                throw new Exception("Source does not inherit from BlockModel.");

            var sourceContent = source.Content;
            var sourceCulture = source.CurrentCulture;

            // destinationType is BlockModel<TContent> else CanConvertTo would have returned false
            var destinationContentType = destinationType.GetGenericArguments()[0];

            if ((sourceContent.GetType().Inherits(destinationContentType)) == false)
                throw new InvalidCastException(string.Format("Cannot cast source content type {0} to destination content type {1}.",
                    sourceContent.GetType(), destinationContentType));

            return Activator.CreateInstance(destinationType, sourceContent, source.Block, sourceCulture);

            //return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
