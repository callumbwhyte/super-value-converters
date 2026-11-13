using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Our.Umbraco.SuperValueConverters.Extensions;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Extensions;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public abstract class SuperValueConverterBase : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
    {
        private readonly IPropertyValueConverter _valueConverter;
        private readonly TypeLoader _typeLoader;

        public SuperValueConverterBase(IPropertyValueConverter valueConverter, TypeLoader typeLoader)
        {
            _valueConverter = valueConverter;
            _typeLoader = typeLoader;
        }

        /// <summary>
        /// Property aliases to ignore, using the Core value converter instead
        /// </summary>
        public virtual string[] IgnoreProperties { get; } = [];

        /// <inheritdoc />
        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            if (IgnoreProperties?.InvariantContains(propertyType.Alias) == true)
            {
                return _valueConverter.GetPropertyValueType(propertyType);
            }

            var settings = GetSettings(propertyType);

            var modelType = settings.DefaultType ?? typeof(IPublishedElement);

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

        private Type? GetTypeForAllowedTypes(string[] allowedTypes)
        {
            var types = _typeLoader.GetTypes<IPublishedElement>()
                .Where(type =>
                {
                    var attribute = type.GetCustomAttribute<PublishedModelAttribute>(false);

                    var modelName = attribute != null ? attribute.ContentTypeAlias : type.Name;

                    return allowedTypes.InvariantContains(modelName);
                });

            if (types.Any() == true)
            {
                if (allowedTypes?.Length > 1)
                {
                    var interfaces = types.Select(x => x
                        .GetInterfaces()
                        .Where(i => i.IsPublic)
                        .Where(i => i != typeof(IPublishedElement)));

                    var sharedInterfaces = interfaces.IntersectMany();

                    return sharedInterfaces.LastOrDefault();
                }
            }

            return types.FirstOrDefault();
        }

        /// <inheritdoc />
        public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
        {
            var value = _valueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

            if (IgnoreProperties?.InvariantContains(propertyType.Alias) == true)
            {
                return value;
            }

            var clrType = propertyType.ClrType;

            var allowsMultiple = clrType?.IsIEnumerable() == true;

            var modelType = allowsMultiple == true ? clrType?.GetInnerType() : clrType;

            var castItems = CastSourceToList(value, modelType);

            return allowsMultiple == true ? castItems : castItems?.FirstOrNull();
        }

        private static IList CastSourceToList(object? source, Type? modelType)
        {
            modelType ??= typeof(IPublishedElement);

            var castItems = modelType.CreateList();

            if (source is IEnumerable items)
            {
                foreach (var item in items)
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
            else if (source is object single)
            {
                if (modelType.IsAssignableFrom(single.GetType()) == true)
                {
                    castItems.Add(single);
                }
            }

            return castItems;
        }

        #region Base overrides

        /// <inheritdoc />
        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        {
            return _valueConverter.GetPropertyCacheLevel(propertyType);
        }

        /// <inheritdoc />
        public override bool? IsValue(object? value, PropertyValueLevel level)
        {
            return _valueConverter.IsValue(value, level);
        }

        /// <inheritdoc />
        public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
        {
            return _valueConverter.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        /// <inheritdoc />
        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return _valueConverter.IsConverter(propertyType);
        }

        #endregion

        #region Delivery API

        // The Delivery API has its own output models, so is left to the Core converter.
        private IDeliveryApiPropertyValueConverter? DeliveryApiConverter => _valueConverter as IDeliveryApiPropertyValueConverter;

        /// <inheritdoc />
        public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType)
        {
            return DeliveryApiConverter?.GetDeliveryApiPropertyCacheLevel(propertyType) ?? GetPropertyCacheLevel(propertyType);
        }

        /// <inheritdoc />
        public PropertyCacheLevel GetDeliveryApiPropertyCacheLevelForExpansion(IPublishedPropertyType propertyType)
        {
            return DeliveryApiConverter?.GetDeliveryApiPropertyCacheLevelForExpansion(propertyType) ?? GetPropertyCacheLevel(propertyType);
        }

        /// <inheritdoc />
        public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType)
        {
            return DeliveryApiConverter?.GetDeliveryApiPropertyValueType(propertyType) ?? _valueConverter.GetPropertyValueType(propertyType);
        }

        /// <inheritdoc />
        public object? ConvertIntermediateToDeliveryApiObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview, bool expanding)
        {
            if (DeliveryApiConverter != null)
            {
                return DeliveryApiConverter.ConvertIntermediateToDeliveryApiObject(owner, propertyType, referenceCacheLevel, inter, preview, expanding);
            }

            return _valueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }

        #endregion

        /// <summary>
        /// Builds the <see cref="IPickerSettings" /> object for the property type
        /// </summary>
        public abstract IPickerSettings GetSettings(IPublishedPropertyType propertyType);
    }
}