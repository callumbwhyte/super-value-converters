using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    internal class MediaPickerValueConverter : SuperValueConverterBase
    {
        public MediaPickerValueConverter(Core.MediaPickerWithCropsValueConverter baseValueConverter, TypeLoader typeLoader)
            : base(baseValueConverter, typeLoader)
        {

        }

        public override IPickerSettings GetSettings(IPublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<MediaPicker3Configuration>();

            var settings = new PickerSettings
            {
                MaxItems = configuration.ValidationLimit.Max ?? 0,
                BaseType = typeof(MediaWithCrops<>)
            };

            if (string.IsNullOrEmpty(configuration.Filter) == false)
            {
                settings.AllowedTypes = configuration.Filter.Split(',');
            }

            return settings;
        }
    }
}