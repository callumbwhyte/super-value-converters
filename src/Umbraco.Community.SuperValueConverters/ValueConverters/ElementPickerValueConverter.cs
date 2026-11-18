using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

[HideFromTypeFinder]
internal class ElementPickerValueConverter<T> : SuperValueConverterBase
    where T : IPropertyValueConverter
{
    private readonly ModelFactory _modelFactory;

    public ElementPickerValueConverter(T valueConverter, ModelFactory modelFactory)
        : base(valueConverter)
    {
        _modelFactory = modelFactory;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
    {
        string? filter = null;

        var multiple = false;

#if NET11_0_OR_GREATER
        if (propertyType.DataType.ConfigurationObject is ElementPickerConfiguration config)
        {
            filter = config.AllowedContentTypeIds;

            multiple = config.ValidationLimit?.Max != 1;
        }
#endif

        var keys = filter?
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => Guid.TryParse(x, out var guid) ? guid : default);

        var itemType = GetItemType(keys ?? []);

        if (multiple == true)
        {
            return typeof(IEnumerable<>).MakeGenericType(itemType);
        }

        return itemType;
    }

    private Type GetItemType(IEnumerable<Guid> keys)
    {
        var modelType = _modelFactory.GetModelType(PublishedItemType.Element, keys);

        if (modelType != null)
        {
            return modelType;
        }

        return typeof(IPublishedElement);
    }
}