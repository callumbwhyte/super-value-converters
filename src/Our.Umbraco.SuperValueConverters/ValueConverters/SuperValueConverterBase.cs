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
                if (ModelsBuilderHelper.IsEnabled() == true)
                {
                    var foundType = GetTypeForAllowedDoctypes(settings.AllowedDoctypes);

                    if (foundType != null)
                    {
                        modelType = foundType;
                    }
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
            var modelsNamespace = ModelsBuilderHelper.GetNamespace();

            var types = TypeHelper.GetTypes(allowedDoctypes, modelsNamespace);

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

            bool allowsMultiple = TypeHelper.IsIEnumerable(clrType);

            var innerType = allowsMultiple ? TypeHelper.GetInnerType(clrType) : clrType;

            var list = TypeHelper.CreateListOfType(innerType);

            var items = GetItemsFromSource(value);

            foreach (var item in items)
            {
                var itemType = item.GetType();

                if (itemType != innerType)
                {
                    if (innerType == typeof(IPublishedContent)
                        && TypeHelper.IsIPublishedContent(itemType) == true)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    list.Add(item);
                }
            }

            return allowsMultiple == true ? list : list.FirstOrNull();
        }

        private static IEnumerable<IPublishedContent> GetItemsFromSource(object source)
        {
            var sourceItems = new List<IPublishedContent>();

            var sourceAsList = source as IEnumerable<IPublishedContent>;

            if (sourceAsList == null)
            {
                var sourceAsSingle = source as IPublishedContent;

                if (sourceAsSingle != null)
                {
                    sourceItems.Add(sourceAsSingle);
                }
            }
            else
            {
                sourceItems.AddRange(sourceAsList);
            }

            return sourceItems;

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