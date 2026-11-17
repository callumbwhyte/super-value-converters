using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    [HideFromTypeFinder]
    internal class ElementPickerValueConverter<T> : SuperValueConverterBase
        where T : IPropertyValueConverter
    {
        private readonly IContentTypeService _contentTypeService;

        public ElementPickerValueConverter(T valueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService)
            : base(valueConverter, typeLoader)
        {
            _contentTypeService = contentTypeService;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            string? filter = null;

            var multiple = false;

#if NET11_0_OR_GREATER
            if (propertyType.DataType.ConfigurationObject is ElementPickerConfiguration config)
            {
                filter = config.AllowedContentTypeIds;

                multiple = config.ValidationLimit?.Max != 1;
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
            var aliases = _contentTypeService
                .GetMany(keys)
                .Select(x => x.Alias)
                .ToArray();

            if (aliases?.Any() == true)
            {
                var modelType = GetPublishedModelType(aliases);

                if (modelType != null)
                {
                    return modelType;
                }
            }

            return typeof(IPublishedElement);
        }
    }
}