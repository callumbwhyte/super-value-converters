using Our.Umbraco.SuperValueConverters.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;
using Core = Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MediaPicker3ValueConverter : SuperValueConverterBase
    {
        public MediaPicker3ValueConverter(Core.MediaPickerWithCropsValueConverter baseValueConverter)
            : base(baseValueConverter)
        {

        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var settings = GetSettings(propertyType);

            var mediaGenericType = GetGenericMediaType(settings);

            var type = settings.AllowsMultiple() ? typeof(IEnumerable<>).MakeGenericType(mediaGenericType) : mediaGenericType;

            return type;
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var media = BaseValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

            if (media == null)
            {
                return media;
            }

            var settings = GetSettings(propertyType);

            var mediaGenericType = GetGenericMediaType(settings);

            if (media is IEnumerable<MediaWithCrops> multiMedia)
            {
                var typedMultiMedia = AdaptAndCast(multiMedia, mediaGenericType);
                return typedMultiMedia;
            }

            if (media is MediaWithCrops)
            {
                var typedMedia = Activator.CreateInstance(mediaGenericType, media);
                return typedMedia;
            }

            return null;
        }

        private IList AdaptAndCast(IEnumerable<MediaWithCrops> media, Type mediaGeneric)
        {
            Type listType = typeof(List<>).MakeGenericType(new[] { mediaGeneric });
            IList list = (IList)Activator.CreateInstance(listType);

            foreach (var m in media)
            {
                var typedMedia = AdaptAndCast(m, mediaGeneric);
                list.Add(typedMedia);
            }

            return list;
        }

        private object AdaptAndCast(MediaWithCrops media, Type mediaGeneric)
        {
            return Convert.ChangeType(Activator.CreateInstance(mediaGeneric, media), mediaGeneric);
        }


        private Type GetGenericMediaType(IPickerSettings settings)
        {
            var mediaWithCropsType = settings.DefaultType ?? typeof(MediaWithCrops);

            if (settings.AllowedTypes.Any() == true)
            {
                var mediaItemType = GetTypeForAllowedTypes(settings.AllowedTypes);

                if (mediaItemType != null)
                {
                    mediaWithCropsType = typeof(MediaWithCrops<>).MakeGenericType(mediaItemType);
                }
            }

            return mediaWithCropsType;
        }

        public override IPickerSettings GetSettings(IPublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<MediaPicker3Configuration>();

            var validationMax = configuration.ValidationLimit?.Max ?? 0;

            var max = configuration.Multiple ? validationMax : 1;

            var settings = new PickerSettings
            {
                MaxItems = max,
                DefaultType = typeof(MediaWithCrops)
            };

            if (string.IsNullOrEmpty(configuration.Filter) == false)
            {
                settings.AllowedTypes = configuration.Filter.Split(',');
            }

            return settings;
        }
    }
}