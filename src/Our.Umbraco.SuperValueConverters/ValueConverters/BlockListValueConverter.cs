using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
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
#if NET11_0_OR_GREATER
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
    }
}