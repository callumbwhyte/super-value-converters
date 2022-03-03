using Our.Umbraco.SuperValueConverters.Composing;
using Our.Umbraco.SuperValueConverters.ValueConverters;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

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