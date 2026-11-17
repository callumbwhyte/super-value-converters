using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Community.SuperValueConverters.ValueConverters;

[HideFromTypeFinder]
internal class BlockListValueConverter<T> : SuperValueConverterBase
    where T : IPropertyValueConverter
{
    private readonly IContentTypeService _contentTypeService;

    public BlockListValueConverter(T valueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService)
        : base(valueConverter, typeLoader)
    {
        _contentTypeService = contentTypeService;
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
        var contentTypeAliases = _contentTypeService
            .GetMany(contentTypeKeys)
            .Select(x => x.Alias)
            .ToArray();

        if (contentTypeAliases?.Any() != true)
        {
            return typeof(BlockListItem);
        }

        var contentModelType = GetPublishedModelType(contentTypeAliases) ?? typeof(IPublishedElement);

        var settingsTypeAliases = _contentTypeService
            .GetMany(settingsTypeKeys)
            .Select(x => x.Alias)
            .ToArray();

        if (settingsTypeAliases?.Any() == true)
        {
            var settingsModelType = GetPublishedModelType(settingsTypeAliases);

            if (settingsModelType != null)
            {
                return typeof(BlockListItem<,>).MakeGenericType(contentModelType, settingsModelType);
            }
        }

        return typeof(BlockListItem<>).MakeGenericType(contentModelType);
    }

    protected override object? ConvertItemToType(object? item, Type type)
    {
        if (item is not BlockListItem block)
        {
            return null;
        }

        var typeArguments = type.GetGenericArguments();

        var contentType = typeArguments.ElementAtOrDefault(0) ?? typeof(IPublishedElement);

        if (contentType.IsInstanceOfType(block.Content) == false)
        {
            return null;
        }

        var settingsType = typeArguments.ElementAtOrDefault(1) ?? typeof(IPublishedElement);

        if (block.Settings != null && !settingsType.IsInstanceOfType(block.Settings))
        {
            return null;
        }

        var constructor = type.GetConstructor([typeof(Guid), contentType, typeof(Guid?), settingsType]);

        return constructor?.Invoke([block.ContentKey, block.Content, block.SettingsKey, block.Settings]);
    }
}