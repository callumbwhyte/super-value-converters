using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Our.Umbraco.SuperValueConverters.PreValues;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MediaPickerValueConverter : SuperValueConverter
    {
        public MediaPickerValueConverter()
            : base(new MediaPickerPropertyConverter())
        {

        }

        public override IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            var preValues = DataTypeHelper.GetPreValues(propertyType.DataTypeId);

            var settings = new MediaPickerSettings();

            return PreValueMapper.Map(settings, preValues);
        }
    }
}