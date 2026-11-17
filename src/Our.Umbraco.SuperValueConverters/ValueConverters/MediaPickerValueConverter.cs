using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    internal class MediaPickerValueConverter : SuperValueConverterBase
    {
        private readonly IMediaTypeService _mediaTypeService;

        public MediaPickerValueConverter(Core.MediaPickerWithCropsValueConverter valueConverter, TypeLoader typeLoader, IMediaTypeService mediaTypeService)
            : base(valueConverter, typeLoader)
        {
            _mediaTypeService = mediaTypeService;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var config = propertyType.DataType.ConfigurationAs<MediaPicker3Configuration>();

            var multiple = config?.ValidationLimit.Max != 1;

            var keys = config?.Filter?
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