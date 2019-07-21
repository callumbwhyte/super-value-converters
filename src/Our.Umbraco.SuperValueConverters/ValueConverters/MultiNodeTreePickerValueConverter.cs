using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PropertyEditors;
using Core = Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : SuperValueConverterBase
    {
        public MultiNodeTreePickerValueConverter(Core.MultiNodeTreePickerValueConverter baseValueConverter)
            : base(baseValueConverter)
        {

        }

        public override IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<MultiNodePickerConfiguration>();

            var settings = new PickerSettings
            {
                AllowedTypes = configuration.Filter.Split(',') ?? new string[] { },
                MaxItems = configuration.MaxNumber
            };

            return settings;
        }
    }
}