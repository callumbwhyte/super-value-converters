using Our.Umbraco.SuperValueConverters.Composing;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters
{
    public class Composer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.DisableConverter<MediaPickerWithCropsValueConverter>();

            builder.DisableConverter<MultiNodeTreePickerValueConverter>();

            builder.DisableConverter<NestedContentManyValueConverter>();

            builder.DisableConverter<NestedContentSingleValueConverter>();
        }
    }
}