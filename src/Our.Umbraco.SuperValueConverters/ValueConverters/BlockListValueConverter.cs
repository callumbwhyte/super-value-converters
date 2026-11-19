using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    internal class BlockListValueConverter : SuperValueConverterBase
    {
        private readonly ModelFactory _modelFactory;

        public BlockListValueConverter(Core.BlockListPropertyValueConverter valueConverter, ModelFactory modelFactory)
            : base(valueConverter, modelFactory)
        {
            _modelFactory = modelFactory;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var config = propertyType.DataType.ConfigurationAs<BlockListConfiguration>();

            var contentTypeKeys = config.Blocks
                .Select(x => x.ContentElementTypeKey)
                .Distinct();

            var settingsTypeKeys = config.Blocks
                .Select(x => x.SettingsElementTypeKey)
                .OfType<Guid>()
                .Distinct();

            var multiple = config.ValidationLimit?.Max != 1;

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

        protected override object ConvertItemToType(object item, Type type)
        {
            if (item is not BlockListItem block)
            {
                return null;
            }

            return _modelFactory.CreateBlockListItem(type, block);
        }
    }
}