using System;
using System.Collections.Generic;
using Our.Umbraco.SuperValueConverters.Helpers;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : MultiNodeTreePickerPropertyConverter, IPropertyValueConverterMeta
    {
        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            return BaseValueConverter.GetPropertyCacheLevel(propertyType, cacheValue);
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            var pickerSettings = DataTypeHelper.GetMNTPSettings(propertyType.DataTypeId);

            return BaseValueConverter.GetPropertyValueType(propertyType, pickerSettings);
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            var value = base.ConvertSourceToObject(propertyType, source, preview) as IEnumerable<IPublishedContent>;

            return BaseValueConverter.ConvertSourceToObject(propertyType, value);
        }
    }
}