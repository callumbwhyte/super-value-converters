using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Our.Umbraco.SuperValueConverters.PreValues;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class NestedContentValueConverter : SuperValueConverter
    {
        public NestedContentValueConverter()
            : base(new NestedContentManyValueConverter())
        {

        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.NestedContentAlias);
        }

        public override IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            var preValues = DataTypeHelper.GetPreValues(propertyType.DataTypeId);

            var settings = new NestedContentSettings();

            return PreValueMapper.Map(settings, preValues);
        }
    }
}