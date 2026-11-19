using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

[HideFromTypeFinder]
internal class BlockListValueConverter<T> : SuperValueConverterBase
    where T : IPropertyValueConverter
{
    private readonly ModelFactory _modelFactory;

    public BlockListValueConverter(T valueConverter, ModelFactory modelFactory)
        : base(valueConverter)
    {
        _modelFactory = modelFactory;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
    {
        BlockListConfiguration.BlockConfiguration[] blocks = [];

        var multiple = false;

        var config = propertyType.DataType.ConfigurationObject;

        if (config is BlockListConfiguration multi)
        {
            blocks = multi.Blocks;

            multiple = multi.ValidationLimit?.Max != 1;
        }
#if NET10_0_OR_GREATER
        else if (config is SingleBlockConfiguration single)
        {
            blocks = single.Blocks;
        }
#endif

        var contentTypeKeys = blocks
            .Select(x => x.ContentElementTypeKey)
            .Distinct();

        var settingsTypeKeys = blocks
            .Where(x => x.SettingsElementTypeKey != null)
            .Select(x => x.SettingsElementTypeKey!.Value)
            .Distinct();

        var blockType = GetBlockType(contentTypeKeys, settingsTypeKeys);

        if (multiple == true)
        {
            return typeof(IEnumerable<>).MakeGenericType(blockType);
        }

        return blockType;
    }

    private Type GetBlockType(IEnumerable<Guid> contentTypeKeys, IEnumerable<Guid> settingsTypeKeys)
    {
        var contentModelType = _modelFactory.GetModelType(PublishedItemType.Element, contentTypeKeys);

        var settingsModelType = _modelFactory.GetModelType(PublishedItemType.Element, settingsTypeKeys);

        if (settingsModelType != null)
        {
            return typeof(BlockListItem<,>).MakeGenericType(contentModelType ?? typeof(IPublishedElement), settingsModelType);
        }
        else if (contentModelType != null)
        {
            return typeof(BlockListItem<>).MakeGenericType(contentModelType);
        }

        return typeof(BlockListItem);
    }

    protected override object? ConvertItemToType(object? item, Type type)
    {
        if (item is not BlockListItem block)
        {
            return null;
        }

        return _modelFactory.CreateBlockListItem(type, block);
    }
}