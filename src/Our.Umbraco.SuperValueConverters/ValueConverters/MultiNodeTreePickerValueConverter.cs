using System;
using System.Collections.Generic;
using System.Linq;
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
            var pickerSettings = DataTypeHelper.GetPickerSettings(propertyType.DataTypeId);

            var modelType = typeof(IPublishedContent);

            if (pickerSettings.AllowedDoctypes.Count() == 1)
            {
                var foundType = TypeHelper.GetType(pickerSettings.AllowedDoctypes.FirstOrDefault());

                if (foundType != null)
                {
                    modelType = foundType;
                }
            }

            if (pickerSettings.AllowsMultiple() == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(modelType);
            }

            return modelType;
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            var data = base.ConvertSourceToObject(propertyType, source, preview) as IEnumerable<IPublishedContent>;

            var clrType = propertyType.ClrType;

            bool allowsMultiple = TypeHelper.IsIEnumerable(clrType);

            if (data != null)
            {
                if (allowsMultiple == true)
                {
                    return data;
                }

                return data.FirstOrDefault();
            }

            return allowsMultiple == true ? Enumerable.Empty<IPublishedContent>() : null;
        }
    }
}