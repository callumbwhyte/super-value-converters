using Our.Umbraco.SuperValueConverters.Composing;
using Umbraco.Core.Composing;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.Startup
{
    public class MultiNodeTreePickerComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.DisableConverter<MultiNodeTreePickerValueConverter>();
        }
    }
}