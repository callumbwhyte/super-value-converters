using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.Composing
{
    public class MultiNodeTreePickerComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.DisableConverter<MultiNodeTreePickerValueConverter>();
        }
    }
}