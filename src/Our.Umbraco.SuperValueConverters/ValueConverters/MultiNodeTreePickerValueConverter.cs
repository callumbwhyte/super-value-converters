using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : MultiNodeTreePickerPropertyConverter
    {
        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            return base.ConvertDataToSource(propertyType, source, preview);
        }
    }
}