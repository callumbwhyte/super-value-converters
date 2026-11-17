using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    [HideFromTypeFinder]
    internal class MediaPickerValueConverter<T> : SuperValueConverterBase
        where T : IPropertyValueConverter
    {
        private readonly IMediaTypeService _mediaTypeService;

        public MediaPickerValueConverter(T valueConverter, TypeLoader typeLoader, IMediaTypeService mediaTypeService)
            : base(valueConverter, typeLoader)
        {
            _mediaTypeService = mediaTypeService;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            string? filter = null;

            var multiple = false;

            var config = propertyType.DataType.ConfigurationObject;

            if (config is MediaPicker3Configuration multi)
            {
                filter = multi.Filter;

                multiple = multi.ValidationLimit?.Max != 1;
            }
#if NET11_0_OR_GREATER
            else if (config is SingleMediaPickerConfiguration single)
            {
                filter = single.Filter;
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
            var aliases = _mediaTypeService
                .GetMany(keys)
                .Select(x => x.Alias)
                .ToArray();

            if (aliases?.Any() == true)
            {
                var modelType = GetPublishedModelType(aliases);

                if (modelType != null)
                {
                    return typeof(MediaWithCrops<>).MakeGenericType(modelType);
                }
            }

            return typeof(MediaWithCrops);
        }
    }
}