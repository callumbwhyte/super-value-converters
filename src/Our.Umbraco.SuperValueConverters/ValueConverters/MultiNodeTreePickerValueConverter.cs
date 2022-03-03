using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : SuperValueConverterBase
    {
        public MultiNodeTreePickerValueConverter(Core.MultiNodeTreePickerValueConverter baseValueConverter, TypeLoader typeLoader)
            : base(baseValueConverter, typeLoader)
        {

        }

        public override IPickerSettings GetSettings(IPublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<MultiNodePickerConfiguration>();

            var settings = new PickerSettings
            {
                MaxItems = configuration.MaxNumber
            };

            if (string.IsNullOrEmpty(configuration.Filter) == false)
            {
                settings.AllowedTypes = configuration.Filter.Split(',');
            }

            return settings;
        }
    }
}