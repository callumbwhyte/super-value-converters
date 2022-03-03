using Our.Umbraco.SuperValueConverters.Composing;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.Composing
{
    public class NestedContentComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.DisableConverter<NestedContentManyValueConverter>();

            builder.DisableConverter<NestedContentSingleValueConverter>();
        }
    }
}