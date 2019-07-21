using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PropertyEditors;
using Core = Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MediaPickerValueConverter : SuperValueConverterBase
    {
        public MediaPickerValueConverter(Core.MediaPickerValueConverter baseValueConverter)
            : base(baseValueConverter)
        {

        }

        public override IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<MediaPickerConfiguration>();

            var settings = new MediaPickerSettings
            {
                AllowedDoctypes = configuration.OnlyImages ? new string[] { "images" } : new string[] { },
                MaxItems = configuration.Multiple ? 1 : 0
            };

            return settings;
        }
    }
}