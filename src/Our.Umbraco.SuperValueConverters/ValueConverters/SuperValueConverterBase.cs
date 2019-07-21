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

        public override Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            var settings = GetSettings(propertyType);

            var modelType = typeof(IPublishedContent);

            if (settings.AllowedTypes.Any() == true)
            {
                if (ModelsBuilderHelper.IsEnabled() == true)
                {
                    var foundType = GetTypeForAllowedTypes(settings.AllowedTypes);

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

        private static Type GetTypeForAllowedTypes(string[] allowedTypes)
        {
            var modelsNamespace = ModelsBuilderHelper.GetNamespace();

            var types = TypeHelper.GetTypes(allowedTypes, modelsNamespace);

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

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var value = _baseValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

            var clrType = propertyType.ClrType;

            bool allowsMultiple = TypeHelper.IsIEnumerable(clrType);

            var modelType = allowsMultiple == true ? TypeHelper.GetInnerType(clrType) : clrType;

            var castItems = CastSourceToList(value, modelType);

            return allowsMultiple == true ? castItems : castItems.FirstOrNull();
        }

        private static IList CastSourceToList(object source, Type modelType)
        {
            var castItems = TypeHelper.CreateListOfType(modelType);

            var sourceAsList = source as IEnumerable<IPublishedContent>;

            if (sourceAsList == null)
            {
                var sourceAsSingle = source as IPublishedContent;

                if (sourceAsSingle != null)
                {
                    if (TypeHelper.IsAssignable(modelType, sourceAsSingle.GetType()) == true)
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
                        if (TypeHelper.IsAssignable(modelType, item.GetType()) == true)
                        {
                            castItems.Add(item);
                        }
                    }
                }
            }

            return castItems;
        }

        #region Base overrides

        public override object ConvertIntermediateToXPath(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            return _baseValueConverter.ConvertIntermediateToXPath(owner, propertyType, referenceCacheLevel, inter, preview);
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview)
        {
            return _baseValueConverter.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return _baseValueConverter.IsConverter(propertyType);
        }

        #endregion

        public abstract IPickerSettings GetSettings(PublishedPropertyType propertyType);
    }
}