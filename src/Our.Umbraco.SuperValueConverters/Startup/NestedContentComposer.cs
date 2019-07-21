using Our.Umbraco.SuperValueConverters.Extensions;
using Umbraco.Core.Composing;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.Startup
{
    public class NestedContentComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.DisableConverter<NestedContentManyValueConverter>();
            composition.DisableConverter<NestedContentSingleValueConverter>();
        }
    }
}