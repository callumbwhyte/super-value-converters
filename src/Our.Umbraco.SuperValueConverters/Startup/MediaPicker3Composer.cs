using Our.Umbraco.SuperValueConverters.Composing;
using Umbraco.Core.Composing;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.Startup
{
    public class MediaPicker3Composer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.DisableConverter<MediaPickerWithCropsValueConverter>();
        }
    }
}