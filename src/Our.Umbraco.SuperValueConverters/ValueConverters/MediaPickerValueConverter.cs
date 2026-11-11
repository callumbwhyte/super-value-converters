using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    internal class MediaPickerValueConverter : SuperValueConverterBase
    {
        public MediaPickerValueConverter(Core.MediaPickerWithCropsValueConverter valueConverter, TypeLoader typeLoader)
            : base(valueConverter, typeLoader)
        {

        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var config = propertyType.DataType.ConfigurationAs<MediaPicker3Configuration>();

            var allowedTypes = config.Filter?.Split(',') ?? new string[0];

            var multiple = config.ValidationLimit.Max != 1;

            var modelType = GetPublishedModelType(allowedTypes);

            var itemType = modelType != null
                ? typeof(MediaWithCrops<>).MakeGenericType(modelType)
                : typeof(MediaWithCrops);

            if (multiple == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(itemType);
            }

            return itemType;
        }
    }
}