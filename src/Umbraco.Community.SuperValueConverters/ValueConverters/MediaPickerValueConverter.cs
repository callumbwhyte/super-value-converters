using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

[HideFromTypeFinder]
internal class MediaPickerValueConverter<T> : SuperValueConverterBase
    where T : IPropertyValueConverter
{
    private readonly ModelFactory _modelFactory;

    public MediaPickerValueConverter(T valueConverter, ModelFactory modelFactory)
        : base(valueConverter)
    {
        _modelFactory = modelFactory;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
    {
        string? filter = null;

        var multiple = false;

        var config = propertyType.DataType.ConfigurationObject;

        if (config is MediaPicker3Configuration multi)
        {
            filter = multi.Filter;

            multiple = multi.ValidationLimit?.Max != 1;
        }
#if NET11_0_OR_GREATER
        else if (config is SingleMediaPickerConfiguration single)
        {
            filter = single.Filter;
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
        var modelType = _modelFactory.GetModelType(PublishedItemType.Media, keys);

        if (modelType != null)
        {
            return typeof(MediaWithCrops<>).MakeGenericType(modelType);
        }

        return typeof(MediaWithCrops);
    }

    protected override object? ConvertItemToType(object? item, Type type)
    {
        if (item is not MediaWithCrops media)
        {
            return null;
        }

        var typeArguments = type.GetGenericArguments();

        var mediaType = typeArguments.ElementAtOrDefault(0) ?? typeof(IPublishedContent);

        if (mediaType.IsInstanceOfType(media.Content) == false)
        {
            return null;
        }

        return _modelFactory.CreateModel(type, media.Content, media.LocalCrops);
    }
}