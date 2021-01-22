using System;
using System.Collections;
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
    public abstract class SuperValueConverterBase : PropertyValueConverterBase, IPropertyValueConverterMeta
    {
        private IPropertyValueConverter _baseValueConverter;

        public SuperValueConverterBase(IPropertyValueConverter baseValueConverter)
        {
            _baseValueConverter = baseValueConverter;
        }

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
            var settings = GetSettings(propertyType);

            var modelType = typeof(IPublishedContent);

            if (settings.AllowedDoctypes.Any() == true)
            {
                var foundType = GetTypeForAllowedDoctypes(settings.AllowedDoctypes);

                if (foundType != null)
                {
                    modelType = foundType;
                }
            }

            if (settings.AllowsMultiple() == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(modelType);
            }

            return modelType;
        }

        private static Type GetTypeForAllowedDoctypes(string[] allowedDoctypes)
        {
            var types = TypeHelper.GetPublishedModelTypes(allowedDoctypes);

            if (types.Any() == true)
            {
                if (allowedDoctypes.Length > 1)
                {
                    var interfaces = types.Select(x => x
                            .GetInterfaces()
                            .Where(i => i.IsPublic));

                    var sharedInterfaces = interfaces.IntersectMany();

                    return sharedInterfaces.LastOrDefault();
                }
            }

            return types.FirstOrDefault();
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            var value = _baseValueConverter.ConvertSourceToObject(propertyType, source, preview);

            var clrType = propertyType.ClrType;

            bool allowsMultiple = clrType.IsIEnumerable();

            var modelType = allowsMultiple == true ? clrType.GetInnerType() : clrType;

            var castItems = CastSourceToList(value, modelType);

            return allowsMultiple == true ? castItems : castItems.FirstOrNull();
        }

        private static IList CastSourceToList(object source, Type modelType)
        {
            var castItems = modelType.CreateList();

            var sourceAsList = source as IEnumerable<IPublishedContent>;

            if (sourceAsList == null)
            {
                var sourceAsSingle = source as IPublishedContent;

                if (sourceAsSingle != null)
                {
                    if (modelType.IsAssignableFrom(sourceAsSingle.GetType()) == true)
                    {
                        castItems.Add(sourceAsSingle);
                    }
                }
            }
            else
            {
                foreach (var item in sourceAsList)
                {
                    if (item != null)
                    {
                        if (modelType.IsAssignableFrom(item.GetType()) == true)
                        {
                            castItems.Add(item);
                        }
                    }
                }
            }

            return castItems;
        }

        #region Base overrides

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            return _baseValueConverter.ConvertDataToSource(propertyType, source, preview);
        }

        public override object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview)
        {
            return _baseValueConverter.ConvertSourceToXPath(propertyType, source, preview);
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return _baseValueConverter.IsConverter(propertyType);
        }

        #endregion

        public abstract IPickerSettings GetSettings(PublishedPropertyType propertyType);
    }
}