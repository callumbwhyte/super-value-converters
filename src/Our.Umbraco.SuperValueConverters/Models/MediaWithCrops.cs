using System;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.SuperValueConverters.Models
{
    public class MediaWithCrops<T> : MediaWithCrops where T : class, IPublishedContent
    {
        public MediaWithCrops()
        {

        }

        public MediaWithCrops(MediaWithCrops mediaWithCrops)
        {
            this.MediaItem = mediaWithCrops.MediaItem as T;
            this.LocalCrops = mediaWithCrops.LocalCrops;
        }

        public new T MediaItem { get; set; }
    }
}
