using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters
{
    public class Boostrapper : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            PropertyValueConvertersResolver.Current.RemoveType<MediaPickerPropertyConverter>();
            PropertyValueConvertersResolver.Current.RemoveType<MultiNodeTreePickerPropertyConverter>();
            PropertyValueConvertersResolver.Current.RemoveType<NestedContentManyValueConverter>();
            PropertyValueConvertersResolver.Current.RemoveType<NestedContentSingleValueConverter>();
        }
    }
}