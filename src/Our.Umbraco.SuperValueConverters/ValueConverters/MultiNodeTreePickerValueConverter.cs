using System;
using System.Collections.Generic;
using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Our.Umbraco.SuperValueConverters.PreValues;
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
            var pickerSettings = GetSettings(propertyType);

            return BaseValueConverter.GetPropertyValueType(propertyType, pickerSettings);
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            var value = base.ConvertSourceToObject(propertyType, source, preview);

            return BaseValueConverter.ConvertSourceToObject(propertyType, value);
        }

        private IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            var preValues = DataTypeHelper.GetPreValues(propertyType.DataTypeId);

            var settings = new MNTPSettings();

            return PreValueMapper.Map(settings, preValues);
        }
    }
}