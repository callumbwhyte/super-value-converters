using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Extensions;
using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class BaseValueConverter
    {
        public static PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
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

        public static Type GetPropertyValueType(PublishedPropertyType propertyType, IPickerSettings pickerSettings)
        {
            var modelType = typeof(IPublishedContent);

            if (pickerSettings.AllowedDoctypes.Count() == 1
                && ModelsBuilderHelper.IsEnabled() == true)
            {
                var modelsNamespace = ModelsBuilderHelper.GetNamespace();

                var foundType = TypeHelper.GetType(pickerSettings.AllowedDoctypes.FirstOrDefault(), modelsNamespace);

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

        public static object ConvertSourceToObject(PublishedPropertyType propertyType, IEnumerable<IPublishedContent> value)
        {
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