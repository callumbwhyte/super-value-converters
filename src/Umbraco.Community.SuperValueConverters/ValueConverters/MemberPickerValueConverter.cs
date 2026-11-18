using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

[HideFromTypeFinder]
internal class MemberPickerValueConverter<T> : SuperValueConverterBase
    where T : IPropertyValueConverter
{
    private readonly ModelFactory _modelFactory;

    public MemberPickerValueConverter(T valueConverter, ModelFactory modelFactory)
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
        if (config is MultipleMemberPickerConfiguration multi)
        {
            filter = multi.Filter;

            multiple = multi.ValidationLimit?.Max != 1;
        }
        else if (config is MemberPickerConfiguration single)
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
        var modelType = _modelFactory.GetModelType(PublishedItemType.Member, keys);

        if (modelType != null)
        {
            return modelType;
        }

        return typeof(IPublishedContent);
    }
}