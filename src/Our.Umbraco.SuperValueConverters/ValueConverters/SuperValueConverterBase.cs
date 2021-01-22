using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Extensions;
using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public abstract class SuperValueConverterBase : PropertyValueConverterBase
    {
        private IPropertyValueConverter _baseValueConverter;

        public SuperValueConverterBase(IPropertyValueConverter baseValueConverter)
        {
            _baseValueConverter = baseValueConverter;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var settings = GetSettings(propertyType);

            var modelType = settings.DefaultType ?? typeof(IPublishedContent);

            if (settings.AllowedTypes.Any() == true)
            {
                var returnType = GetTypeForAllowedTypes(settings.AllowedTypes);

                if (returnType != null)
                {
                    modelType = returnType;
                }
            }

            if (settings.AllowsMultiple() == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(modelType);
            }

            return modelType;
        }

        private static Type GetTypeForAllowedTypes(string[] allowedTypes)
        {
            var types = TypeHelper.GetTypes(allowedTypes);

            if (types.Any() == true)
            {
                if (allowedTypes.Length > 1)
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

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var value = _baseValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

            var clrType = propertyType.ClrType;

            bool allowsMultiple = clrType.IsIEnumerable();

            var modelType = allowsMultiple == true ? clrType.GetInnerType() : clrType;

            var castItems = CastSourceToList(value, modelType);

            return allowsMultiple == true ? castItems : castItems.FirstOrNull();
        }

        private static IList CastSourceToList(object source, Type modelType)
        {
            var castItems = modelType.CreateList();

            var sourceAsList = source as IEnumerable<IPublishedElement>;

            if (sourceAsList == null)
            {
                var sourceAsSingle = source as IPublishedElement;

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

        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            return _baseValueConverter.ConvertIntermediateToXPath(owner, propertyType, referenceCacheLevel, inter, preview);
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            return _baseValueConverter.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return _baseValueConverter.IsConverter(propertyType);
        }

        #endregion

        public abstract IPickerSettings GetSettings(IPublishedPropertyType propertyType);
    }
}