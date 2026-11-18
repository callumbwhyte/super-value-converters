using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
#if NET10_0_OR_GREATER
using Umbraco.Cms.Infrastructure.PropertyEditors.ValueConverters;
#endif

namespace Umbraco.Community.SuperValueConverters;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<ModelFactory>();

        Register<BlockListPropertyValueConverter, ValueConverters.BlockListValueConverter<BlockListPropertyValueConverter>>(builder);

#if NET10_0_OR_GREATER
        Register<SingleBlockPropertyValueConverter, ValueConverters.BlockListValueConverter<SingleBlockPropertyValueConverter>>(builder);
#endif

        Register<ContentPickerValueConverter, ValueConverters.ContentPickerValueConverter<ContentPickerValueConverter>>(builder);

#if NET11_0_OR_GREATER
        Register<MultipleDocumentPickerValueConverter, ValueConverters.ContentPickerValueConverter<MultipleDocumentPickerValueConverter>>(builder);
#endif

#if NET11_0_OR_GREATER
        Register<ElementPickerValueConverter, ValueConverters.ElementPickerValueConverter<ElementPickerValueConverter>>(builder);
#endif

        Register<MediaPickerWithCropsValueConverter, ValueConverters.MediaPickerValueConverter<MediaPickerWithCropsValueConverter>>(builder);

#if NET11_0_OR_GREATER
        Register<SingleMediaPickerValueConverter, ValueConverters.MediaPickerValueConverter<SingleMediaPickerValueConverter>>(builder);
#endif

        Register<MemberPickerValueConverter, ValueConverters.MemberPickerValueConverter<MemberPickerValueConverter>>(builder);

#if NET11_0_OR_GREATER
        Register<MultipleMemberPickerValueConverter, ValueConverters.MemberPickerValueConverter<MultipleMemberPickerValueConverter>>(builder);
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