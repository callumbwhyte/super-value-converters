using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core;
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

        public override string[] IgnoreProperties => new[]
        {
            Constants.Conventions.Content.InternalRedirectId,
            Constants.Conventions.Content.Redirect
        };

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