using System.Linq;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class NestedContentValueConverter : SuperValueConverterBase
    {
        public NestedContentValueConverter(Core.NestedContentManyValueConverter baseValueConverter, TypeLoader typeLoader)
            : base(baseValueConverter, typeLoader)
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