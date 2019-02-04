using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MediaPickerValueConverter : MediaPickerPropertyConverter, IPropertyValueConverterMeta
    {
        public new PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            return BaseValueConverter.GetPropertyCacheLevel(propertyType, cacheValue);
        }

        public new Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            var pickerSettings = GetSettings(propertyType);

            return BaseValueConverter.GetPropertyValueType(propertyType, pickerSettings);
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            var value = base.ConvertSourceToObject(propertyType, source, preview) as IEnumerable<IPublishedContent>;

            return BaseValueConverter.ConvertSourceToObject(propertyType, value);
        }

        private IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            var preValues = DataTypeHelper.GetPreValues(propertyType.DataTypeId);

            if (preValues.Any() == true)
            {
                var allowedDoctypes = new List<string>();

                if (preValues["onlyImages"] == "1")
                {
                    allowedDoctypes.Add("Image");
                }

                var maxItems = preValues["multiPicker"] == "1" ? 0 : 1;

                return new MediaPickerSettings
                {
                    AllowedDoctypes = allowedDoctypes.ToArray(),
                    MaxItems = maxItems
                };
            }

            return null;
        }
    }
}