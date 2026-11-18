using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

[HideFromTypeFinder]
internal class ContentPickerValueConverter<T> : SuperValueConverterBase
    where T : IPropertyValueConverter
{
    private readonly ModelFactory _modelFactory;

    public ContentPickerValueConverter(T valueConverter, ModelFactory modelFactory)
        : base(valueConverter)
    {
        _modelFactory = modelFactory;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
    {
        string? filter = null;

        var multiple = false;

        var config = propertyType.DataType.ConfigurationObject;

#if NET11_0_OR_GREATER
        if (config is MultipleDocumentPickerConfiguration multi)
        {
            filter = multi.AllowedContentTypeIds;

            multiple = multi.ValidationLimit?.Max != 1;
        }
        else if (config is ContentPickerConfiguration single)
        {
            filter = single.AllowedContentTypeIds;
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
        var modelType = _modelFactory.GetModelType(PublishedItemType.Content, keys);

        if (modelType != null)
        {
            return modelType;
        }

        return typeof(IPublishedContent);
    }
}