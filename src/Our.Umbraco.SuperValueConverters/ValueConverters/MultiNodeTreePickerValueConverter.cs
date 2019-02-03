using System;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : MultiNodeTreePickerPropertyConverter, IPropertyValueConverterMeta
    {
        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            PropertyCacheLevel returnLevel;

            switch (cacheValue)
            {
                case PropertyCacheValue.Object:
                    returnLevel = PropertyCacheLevel.ContentCache;
                    break;
                case PropertyCacheValue.Source:
                    returnLevel = PropertyCacheLevel.Content;
                    break;
                case PropertyCacheValue.XPath:
                    returnLevel = PropertyCacheLevel.Content;
                    break;
                default:
                    returnLevel = PropertyCacheLevel.None;
                    break;
            }

            return returnLevel;
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            throw new NotImplementedException();
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            return base.ConvertDataToSource(propertyType, source, preview);
        }
    }
}