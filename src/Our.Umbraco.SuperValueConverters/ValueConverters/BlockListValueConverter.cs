using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class BlockListValueConverter : SuperValueConverterBase
    {
        private static readonly ConcurrentDictionary<Type, ConstructorInfo> ItemConstructors = new ConcurrentDictionary<Type, ConstructorInfo>();

        private readonly Core.BlockListPropertyValueConverter _baseValueConverter;
        private readonly IContentTypeService _contentTypeService;

        public BlockListValueConverter(Core.BlockListPropertyValueConverter baseValueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService)
            : base(baseValueConverter, typeLoader)
        {
            _baseValueConverter = baseValueConverter;
            _contentTypeService = contentTypeService;
        }

        public override IPickerSettings GetSettings(IPublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<BlockListConfiguration>();

            return new PickerSettings
            {
                MaxItems = configuration.ValidationLimit?.Max ?? 0,
                AllowedTypes = GetAliases(configuration.Blocks.Select(x => x.ContentElementTypeKey))
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

            var isModel = typeof(BlockListModel).IsAssignableFrom(clrType);

            var typeArguments = clrType.GetGenericArguments();
            var contentType = typeArguments[0];
            var settingsType = typeArguments.Length > 1 ? typeArguments[1] : null;

            var itemType = isModel == true ? GetItemType(contentType, settingsType) : clrType;

            var castItems = itemType.CreateList();

            foreach (var blockItem in items)
            {
                if (contentType.IsInstanceOfType(blockItem.Content) == true)
                {
                    castItems.Add(itemType.IsInstanceOfType(blockItem) == true ? blockItem : CreateItem(itemType, settingsType, blockItem));
                }
            }

            if (isModel == true)
            {
                return Activator.CreateInstance(clrType, castItems);
            }

            return castItems.FirstOrNull();
        }

        private Type GetSettingsType(IPublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<BlockListConfiguration>();

            if (configuration.Blocks.Any(x => x.SettingsElementTypeKey == null) == true)
            {
                return null;
            }

            return GetTypeForAllowedTypes(GetAliases(configuration.Blocks.Select(x => x.SettingsElementTypeKey.Value)));
        }

        private string[] GetAliases(IEnumerable<Guid> keys)
        {
            return keys
                .Select(key => _contentTypeService.Get(key)?.Alias)
                .Where(alias => alias != null)
                .Distinct()
                .ToArray();
        }

        private static Type GetItemType(Type contentType, Type settingsType)
        {
            if (settingsType == null)
            {
                return typeof(BlockListItem<>).MakeGenericType(contentType);
            }

            return typeof(BlockListItem<,>).MakeGenericType(contentType, settingsType);
        }

        // Core types each item by its own block, so a shared interface needs a new item.
        private static BlockListItem CreateItem(Type itemType, Type settingsType, BlockListItem item)
        {
            var constructor = ItemConstructors.GetOrAdd(itemType, type => type.GetConstructor(new[]
            {
                typeof(Guid),
                type.GetGenericArguments()[0],
                typeof(Guid?),
                settingsType ?? typeof(IPublishedElement)
            }));

            var settings = settingsType == null || settingsType.IsInstanceOfType(item.Settings) == true ? item.Settings : null;

            return (BlockListItem)constructor.Invoke(new object[] { item.ContentKey, item.Content, item.SettingsKey, settings });
        }
    }
}
