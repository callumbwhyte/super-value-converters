using System;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters
{
    public partial class ModelFactory
    {
        /// <summary>
        /// Creates a typed instance of <see cref="MediaWithCrops{T}"/>
        /// </summary>
        internal object CreateMediaWithCrops(Type type, MediaWithCrops media)
        {
            var typeArguments = type.GetGenericArguments();

            var mediaType = typeArguments.ElementAtOrDefault(0) ?? typeof(IPublishedContent);

            if (mediaType.IsInstanceOfType(media.Content) == false)
            {
                return null;
            }

            return CreateModel(type, new (Type, object)[]
            {
                (mediaType, media.Content),
                (typeof(ImageCropperValue), media.LocalCrops)
            });
        }
    }
}