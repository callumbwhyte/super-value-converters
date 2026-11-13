using System;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.SuperValueConverters.Models
{
    internal class PickerSettings : IPickerSettings
    {
        public string[] AllowedTypes { get; set; } = [];

        public int MaxItems { get; set; }

        public Type DefaultType { get; set; } = typeof(IPublishedElement);

        public bool AllowsMultiple()
        {
            return MaxItems == 0 || MaxItems > 1;
        }
    }
}