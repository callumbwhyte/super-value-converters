using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Extensions;
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
            var value = base.ConvertSourceToObject(propertyType, source, preview) as IEnumerable<IPublishedContent>;

            var clrType = propertyType.ClrType;

            bool allowsMultiple = TypeHelper.IsIEnumerable(clrType);

            var innerType = allowsMultiple ? TypeHelper.GetInnerType(clrType) : clrType;

            var list = innerType == typeof(IPublishedContent)
                ? new List<IPublishedContent>()
                : TypeHelper.CreateListOfType(innerType);

            if (value != null)
            {
                foreach (var item in value)
                {
                    var itemType = item.GetType();

                    if (itemType == innerType)
                    {
                        list.Add(item);
                    }
                    else if (innerType == typeof(IPublishedContent)
                        && TypeHelper.IsIPublishedContent(itemType) == true)
                    {
                        list.Add(item);
                    }
                }
            }

            return allowsMultiple == true ? list : list.FirstOrNull();
        }
    }
}