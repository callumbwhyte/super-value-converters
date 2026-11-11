using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Extensions;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class BlockListValueConverter : BlockValueConverterBase
    {
        private readonly Core.BlockListPropertyValueConverter _baseValueConverter;

        public BlockListValueConverter(Core.BlockListPropertyValueConverter baseValueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService)
            : base(baseValueConverter, typeLoader, contentTypeService)
        {
            _baseValueConverter = baseValueConverter;
        }

        public override IPickerSettings GetSettings(IPublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<BlockListConfiguration>();

            return new PickerSettings
            {
                MaxItems = configuration.ValidationLimit?.Max ?? 0,
                AllowedTypes = GetContentAliases(propertyType)
            };
        }

        /// <inheritdoc />
        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var settings = GetSettings(propertyType);

            var contentType = GetTypeForAllowedTypes(settings.AllowedTypes);

            if (contentType == null)
            {
                return settings.AllowsMultiple() == true ? typeof(BlockListModel) : typeof(BlockListItem);
            }

            var settingsType = GetSettingsType(propertyType);

            if (settings.AllowsMultiple() == true)
            {
                if (settingsType == null)
                {
                    return typeof(BlockListModel<>).MakeGenericType(contentType);
                }

                return typeof(BlockListModel<,>).MakeGenericType(contentType, settingsType);
            }

            return GetItemType(contentType, settingsType);
        }

        /// <inheritdoc />
        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var value = _baseValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

            var clrType = propertyType.ClrType;

            if (clrType == typeof(BlockListModel) && value is BlockListModel)
            {
                return value;
            }

            // Core returns a single item, not a model, only in single block mode with one block type and a minimum and maximum of 1.
            var items = new List<BlockListItem>();

            if (value is BlockListModel model)
            {
                items.AddRange(model);
            }
            else if (value is BlockListItem item)
            {
                items.Add(item);
            }

            if (clrType == typeof(BlockListModel))
            {
                return new BlockListModel(items);
            }

            if (clrType == typeof(BlockListItem))
            {
                return items.FirstOrDefault();
            }

            if (typeof(BlockListModel).IsAssignableFrom(clrType) == true)
            {
                var typeArguments = clrType.GetGenericArguments();

                var itemType = GetItemType(typeArguments[0], typeArguments.Length > 1 ? typeArguments[1] : null);

                return Activator.CreateInstance(clrType, CastItems(items, itemType));
            }

            return CastItems(items, clrType).FirstOrNull();
        }

        protected override IEnumerable<IBlockConfiguration> GetBlocks(IPublishedPropertyType propertyType)
        {
            return propertyType.DataType.ConfigurationAs<BlockListConfiguration>().Blocks;
        }
    }
}
