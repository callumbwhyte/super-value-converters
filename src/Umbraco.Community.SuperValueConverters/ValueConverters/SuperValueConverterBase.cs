using System.Collections;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Community.SuperValueConverters.Extensions;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

public abstract class SuperValueConverterBase : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
{
    private readonly IPropertyValueConverter _valueConverter;
    private readonly TypeLoader _typeLoader;

    public SuperValueConverterBase(IPropertyValueConverter valueConverter, TypeLoader typeLoader)
    {
        _valueConverter = valueConverter;
        _typeLoader = typeLoader;
    }

    /// <inheritdoc />
    public override bool IsConverter(IPublishedPropertyType propertyType)
    {
        return _valueConverter.IsConverter(propertyType);
    }

    /// <inheritdoc />
    public override bool? IsValue(object? value, PropertyValueLevel level)
    {
        return _valueConverter.IsValue(value, level);
    }

    /// <inheritdoc />
    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
    {
        return _valueConverter.GetPropertyCacheLevel(propertyType);
    }

    /// <inheritdoc />
    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
    {
        return _valueConverter.ConvertSourceToIntermediate(owner, propertyType, source, preview);
    }

    /// <inheritdoc />
    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
    {
        var value = _valueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

        var modelType = propertyType.ClrType ?? GetPropertyValueType(propertyType);

        var multiple = modelType.IsIEnumerable() == true;

        var itemType = multiple == true ? modelType.GetInnerType() : modelType;

        var result = itemType.CreateList();

        if (value is IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item != null)
                {
                    if (itemType.IsAssignableFrom(item.GetType()) == true)
                    {
                        result.Add(item);
                    }
                }
            }
        }
        else if (value is object single)
        {
            if (itemType.IsAssignableFrom(single.GetType()) == true)
            {
                result.Add(single);
            }
        }

        return multiple == true ? result : result?.FirstOrNull();
    }

    /// <summary>
    /// Gets the common published model type for the given content type aliases
    /// </summary>
    protected virtual Type? GetPublishedModelType(params string[] contentTypeAliases)
    {
        var types = _typeLoader.GetPublishedModelTypes(contentTypeAliases);

        return _typeLoader.FindCommonInterfaces(types, type =>
        {
            return type.Namespace != typeof(IPublishedElement).Namespace;
        });
    }

    #region Delivery API

    // The Delivery API has its own output models, so is left to the Core converter.
    private IDeliveryApiPropertyValueConverter? DeliveryApiConverter => _valueConverter as IDeliveryApiPropertyValueConverter;

    /// <inheritdoc />
    public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType)
    {
        return DeliveryApiConverter?.GetDeliveryApiPropertyCacheLevel(propertyType) ?? _valueConverter.GetPropertyCacheLevel(propertyType);
    }

    /// <inheritdoc />
    public PropertyCacheLevel GetDeliveryApiPropertyCacheLevelForExpansion(IPublishedPropertyType propertyType)
    {
        return DeliveryApiConverter?.GetDeliveryApiPropertyCacheLevelForExpansion(propertyType) ?? _valueConverter.GetPropertyCacheLevel(propertyType);
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
}