using Our.Umbraco.SuperValueConverters.ValueConverters;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Core = Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.Startup
{
    public class MediaPickerComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.PropertyValueConverters()
                .Replace<Core.MediaPickerValueConverter, MediaPickerValueConverter>();
        }
    }
}