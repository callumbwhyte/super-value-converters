using System;
using System.Collections.Generic;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Core = Umbraco.Cms.Infrastructure.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class SingleBlockValueConverter : BlockValueConverterBase
    {
        private readonly Core.SingleBlockPropertyValueConverter _baseValueConverter;

        public SingleBlockValueConverter(Core.SingleBlockPropertyValueConverter baseValueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService)
            : base(baseValueConverter, typeLoader, contentTypeService)
        {
            _baseValueConverter = baseValueConverter;
        }

        public override IPickerSettings GetSettings(IPublishedPropertyType propertyType)
        {
            return new PickerSettings
            {
                MaxItems = 1,
                AllowedTypes = GetContentAliases(propertyType)
            };
        }

        /// <inheritdoc />
        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var contentType = GetTypeForAllowedTypes(GetSettings(propertyType).AllowedTypes);

            if (contentType == null)
            {
                return typeof(BlockListItem);
            }

            return GetItemType(contentType, GetSettingsType(propertyType));
        }

        /// <inheritdoc />
        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var value = _baseValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

            var clrType = propertyType.ClrType;

            if (clrType != typeof(BlockListItem) && value is BlockListItem item)
            {
                return CastItem(item, clrType);
            }

            return value;
        }

        protected override IEnumerable<IBlockConfiguration> GetBlocks(IPublishedPropertyType propertyType)
        {
            return propertyType.DataType.ConfigurationAs<SingleBlockConfiguration>().Blocks;
        }
    }
}
