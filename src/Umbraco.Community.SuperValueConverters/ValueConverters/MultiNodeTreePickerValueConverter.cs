using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

[HideFromTypeFinder]
internal class MultiNodeTreePickerValueConverter<T> : SuperValueConverterBase
    where T : IPropertyValueConverter
{
    private readonly ModelFactory _modelFactory;

    public MultiNodeTreePickerValueConverter(T valueConverter, ModelFactory modelFactory)
        : base(valueConverter)
    {
        _modelFactory = modelFactory;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
    {
        var config = propertyType.DataType.ConfigurationAs<MultiNodePickerConfiguration>();

#if NET11_0_OR_GREATER
        var multiple = config?.ValidationLimit?.Max != 1;
#else
        var multiple = config?.MaxNumber != 1;
#endif

        var keys = config?.Filter?
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => Guid.TryParse(x, out var guid) ? guid : default);

        var itemType = GetItemType(config?.TreeSource?.ObjectType, keys ?? []);

        if (multiple == true)
        {
            return typeof(IEnumerable<>).MakeGenericType(itemType);
        }

        return itemType;
    }

    private Type GetItemType(string? objectType, IEnumerable<Guid> keys)
    {
        var itemType = objectType?.ToLowerInvariant() switch
        {
            "content" => PublishedItemType.Content,
            "document" => PublishedItemType.Content,
            "media" => PublishedItemType.Media,
            "member" => PublishedItemType.Member,
            _ => throw new NotSupportedException($"The object type '{objectType}' is not supported.")
        };

        var modelType = _modelFactory.GetModelType(itemType, keys);

        if (modelType != null)
        {
            return modelType;
        }

        return typeof(IPublishedContent);
    }
}