using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

[HideFromTypeFinder]
internal class ContentPickerValueConverter<T> : SuperValueConverterBase
    where T : IPropertyValueConverter
{
    private readonly IContentTypeService _contentTypeService;

    public ContentPickerValueConverter(T valueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService)
        : base(valueConverter, typeLoader)
    {
        _contentTypeService = contentTypeService;
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
        var aliases = _contentTypeService
            .GetMany(keys)
            .Select(x => x.Alias)
            .ToArray();

        if (aliases?.Any() == true)
        {
            var modelType = GetPublishedModelType(aliases);

            if (modelType != null)
            {
                return modelType;
            }
        }

        return typeof(IPublishedContent);
    }
}