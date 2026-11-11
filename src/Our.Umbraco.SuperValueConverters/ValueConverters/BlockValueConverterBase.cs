using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Our.Umbraco.SuperValueConverters.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public abstract class BlockValueConverterBase : SuperValueConverterBase
    {
        private static readonly ConcurrentDictionary<Type, ConstructorInfo> ItemConstructors = new ConcurrentDictionary<Type, ConstructorInfo>();

        private readonly IContentTypeService _contentTypeService;

        public BlockValueConverterBase(IPropertyValueConverter baseValueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService)
            : base(baseValueConverter, typeLoader)
        {
            _contentTypeService = contentTypeService;
        }

        protected abstract IEnumerable<IBlockConfiguration> GetBlocks(IPublishedPropertyType propertyType);

        protected string[] GetContentAliases(IPublishedPropertyType propertyType)
        {
            return GetAliases(GetBlocks(propertyType).Select(x => x.ContentElementTypeKey));
        }

        protected Type GetSettingsType(IPublishedPropertyType propertyType)
        {
            var blocks = GetBlocks(propertyType).ToArray();

            if (blocks.Any(x => x.SettingsElementTypeKey == null) == true)
            {
                return null;
            }

            return GetTypeForAllowedTypes(GetAliases(blocks.Select(x => x.SettingsElementTypeKey.Value)));
        }

        protected static Type GetItemType(Type contentType, Type settingsType)
        {
            if (settingsType == null)
            {
                return typeof(BlockListItem<>).MakeGenericType(contentType);
            }

            return typeof(BlockListItem<,>).MakeGenericType(contentType, settingsType);
        }

        /// <returns>Null if the item's content is not of the item type's content type</returns>
        protected static BlockListItem CastItem(BlockListItem item, Type itemType)
        {
            var typeArguments = itemType.GetGenericArguments();
            var contentType = typeArguments[0];
            var settingsType = typeArguments.Length > 1 ? typeArguments[1] : null;

            if (contentType.IsInstanceOfType(item.Content) == false)
            {
                return null;
            }

            if (itemType.IsInstanceOfType(item) == true)
            {
                return item;
            }

            return CreateItem(itemType, settingsType, item);
        }

        protected static IList CastItems(IEnumerable<BlockListItem> items, Type itemType)
        {
            var castItems = itemType.CreateList();

            foreach (var item in items)
            {
                var castItem = CastItem(item, itemType);

                if (castItem != null)
                {
                    castItems.Add(castItem);
                }
            }

            return castItems;
        }

        private string[] GetAliases(IEnumerable<Guid> keys)
        {
            return keys
                .Select(key => _contentTypeService.Get(key)?.Alias)
                .Where(alias => alias != null)
                .Distinct()
                .ToArray();
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
