using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    internal class BlockListValueConverter : SuperValueConverterBase
    {
        private readonly IContentTypeService _contentTypeService;

        public BlockListValueConverter(Core.BlockListPropertyValueConverter valueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService)
            : base(valueConverter, typeLoader)
        {
            _contentTypeService = contentTypeService;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var config = propertyType.DataType.ConfigurationAs<BlockListConfiguration>();

            var contentTypeKeys = config?.Blocks?
                .Select(x => x.ContentElementTypeKey)
                .Distinct();

            var settingsTypeKeys = config?.Blocks?
                .Where(x => x.SettingsElementTypeKey != null)
                .Select(x => x.SettingsElementTypeKey!.Value)
                .Distinct();

            var multiple = config?.ValidationLimit.Max != 1;

            var blockType = GetBlockType(contentTypeKeys ?? [], settingsTypeKeys ?? []);

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