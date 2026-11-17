using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    [HideFromTypeFinder]
    internal class MultiNodeTreePickerValueConverter<T> : SuperValueConverterBase
        where T : IPropertyValueConverter
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IMemberTypeService _memberTypeService;

        public MultiNodeTreePickerValueConverter(T valueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService, IMediaTypeService mediaTypeService, IMemberTypeService memberTypeService)
            : base(valueConverter, typeLoader)
        {
            _contentTypeService = contentTypeService;
            _mediaTypeService = mediaTypeService;
            _memberTypeService = memberTypeService;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var config = propertyType.DataType.ConfigurationAs<MultiNodePickerConfiguration>();

#if NET11_0_OR_GREATER
            var multiple = config?.ValidationLimit?.Max != 1;
#else
            var multiple = config?.MaxNumber != 1;
#endif

            var keys = config?.Filter?
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Guid.TryParse(x, out var guid) ? guid : default);

            var itemType = GetItemType(config?.TreeSource?.ObjectType, keys ?? []);

            if (multiple == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(itemType);
            }

            return itemType;
        }

        private Type GetItemType(string? objectType, IEnumerable<Guid> keys)
        {
            IEnumerable<IContentTypeBase> allowedTypes = objectType switch
            {
                Constants.UdiEntityType.Document => _contentTypeService.GetMany(keys),
                Constants.UdiEntityType.Media => _mediaTypeService.GetMany(keys),
                Constants.UdiEntityType.Member => _memberTypeService.GetMany(keys),
                _ => throw new NotSupportedException($"The object type '{objectType}' is not supported.")
            };

            var aliases = allowedTypes
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

            return typeof(IPublishedContent);
        }
    }
}