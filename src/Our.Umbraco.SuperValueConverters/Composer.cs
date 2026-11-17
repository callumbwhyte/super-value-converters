using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
#if NET10_0_OR_GREATER
using Umbraco.Cms.Infrastructure.PropertyEditors.ValueConverters;
#endif

namespace Our.Umbraco.SuperValueConverters
{
    public class Composer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            Register<BlockListPropertyValueConverter, ValueConverters.BlockListValueConverter<BlockListPropertyValueConverter>>(builder);

#if NET10_0_OR_GREATER
            Register<SingleBlockPropertyValueConverter, ValueConverters.BlockListValueConverter<SingleBlockPropertyValueConverter>>(builder);
#endif

            Register<MediaPickerWithCropsValueConverter, ValueConverters.MediaPickerValueConverter<MediaPickerWithCropsValueConverter>>(builder);

#if NET11_0_OR_GREATER
            Register<SingleMediaPickerValueConverter, ValueConverters.MediaPickerValueConverter<SingleMediaPickerValueConverter>>(builder);
#endif

            Register<MultiNodeTreePickerValueConverter, ValueConverters.MultiNodeTreePickerValueConverter<MultiNodeTreePickerValueConverter>>(builder);
        }

        private void Register<TSource, TTarget>(IUmbracoBuilder builder)
            where TSource : class, IPropertyValueConverter
            where TTarget : class, IPropertyValueConverter
        {
            builder.PropertyValueConverters().Remove<TSource>();

            builder.PropertyValueConverters().Append<TTarget>();

            builder.Services.AddSingleton<TSource>();
        }
    }
}