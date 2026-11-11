using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.Composing
{
    public class SingleBlockComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.DisableConverter<SingleBlockPropertyValueConverter>();
        }
    }
}
