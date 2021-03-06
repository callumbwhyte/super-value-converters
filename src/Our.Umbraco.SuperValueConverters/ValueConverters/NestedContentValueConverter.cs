﻿using System.Linq;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PropertyEditors;
using Core = Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class NestedContentValueConverter : SuperValueConverterBase
    {
        public NestedContentValueConverter(Core.NestedContentManyValueConverter baseValueConverter)
            : base(baseValueConverter)
        {

        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals(Constants.PropertyEditors.Aliases.NestedContent);
        }

        public override IPickerSettings GetSettings(IPublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<NestedContentConfiguration>();

            var settings = new PickerSettings
            {
                MaxItems = configuration.MaxItems ?? 0,
                DefaultType = typeof(IPublishedElement)
            };

            if (configuration.ContentTypes != null)
            {
                settings.AllowedTypes = configuration.ContentTypes
                    .Select(x => x.Alias)
                    .ToArray();
            }

            return settings;
        }
    }
}