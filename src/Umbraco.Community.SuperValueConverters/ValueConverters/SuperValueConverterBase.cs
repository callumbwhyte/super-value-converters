using System.Collections;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Community.SuperValueConverters.Extensions;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

public abstract class SuperValueConverterBase : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
{
    private readonly IPropertyValueConverter _valueConverter;

    public SuperValueConverterBase(IPropertyValueConverter valueConverter)
    {
        _valueConverter = valueConverter;
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

        var itemType = multiple ? modelType.GetInnerType() : modelType;

        var result = itemType.CreateList();

        var items = value as IEnumerable ?? new[] { value };

        foreach (var item in items)
        {
            if (itemType.IsAssignableFrom(item?.GetType()) == true)
            {
                result.Add(item);
            }
            else
            {
                var converted = ConvertItemToType(item, itemType);

                if (converted != null)
                {
                    result.Add(converted);
                }
            }
        }

        return multiple ? result : result?.FirstOrNull();
    }

    /// <summary>
    /// Converts an individual item to the given type
    /// </summary>
    protected virtual object? ConvertItemToType(object? item, Type type) => null;

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