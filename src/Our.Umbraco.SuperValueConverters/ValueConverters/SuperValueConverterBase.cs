using System;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public abstract class SuperValueConverter : PropertyValueConverterBase, IPropertyValueConverterMeta
    {
        private IPropertyValueConverter _baseValueConverter;

        public SuperValueConverter(IPropertyValueConverter baseValueConverter)
        {
            _baseValueConverter = baseValueConverter;
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            return BaseValueConverter.GetPropertyCacheLevel(propertyType, cacheValue);
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            var settings = GetSettings(propertyType);

            return BaseValueConverter.GetPropertyValueType(propertyType, settings);
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            var value = _baseValueConverter.ConvertSourceToObject(propertyType, source, preview);

            return BaseValueConverter.ConvertSourceToObject(propertyType, source);
        }

        public virtual IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            throw new NotImplementedException();
        }
    }
}